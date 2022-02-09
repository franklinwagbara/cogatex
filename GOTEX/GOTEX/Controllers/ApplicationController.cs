using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using GOTEX.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Rotativa.AspNetCore;

namespace GOTEX.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly AppDbContext _context;
        private IAppConfiguration<Configuration> _appConfig;
        private IApplication<Application> _application;
        private IRepository<Message> _message;
        private IElpsRepository _elps;
        private IAppHistory<ApplicationHistory> _history;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _hostingEnvironment;
        private IPermit<Permit> _permit;
        private IRepository<Log> _log;
        private readonly EmailSettings _emailSettings;
        
        public ApplicationController(
            AppDbContext context,
            IAppConfiguration<Configuration> appConfig,
            IApplication<Application> application,
            UserManager<ApplicationUser> userManager,
            IRepository<Message> message,
            IWebHostEnvironment hostingEnvironment,
            IAppHistory<ApplicationHistory> history,
            IElpsRepository elps,
            IPermit<Permit> permit,
            IRepository<Log> log,
            IOptionsMonitor<EmailSettings> optionsMonitor)
        {
            _context = context;
            _userManager = userManager;
            _appConfig = appConfig;
            _application = application;
            _message = message;
            _hostingEnvironment = hostingEnvironment;
            _history = history;
            _elps = elps;
            _permit = permit;
            _log = log;
            _emailSettings = optionsMonitor.CurrentValue;
        }
        // GET
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (!user.ProfileComplete)
                return RedirectToAction("CompanyProfile", "DashBoard", new{ userid = User.Identity.Name });
            
            ViewBag.Error = TempData["Error"];
            ViewBag.ApplicationTypes = _appConfig.GetApplicationTypes();
            var quarters = _appConfig.GetQuarters().Stringify().Parse<List<Quarter>>();
            
            quarters = LatePaymentText(quarters);
            
            ViewBag.Quarters = quarters;
            ViewBag.Terminals = _appConfig.GetTerminal().Stringify().Parse<List<Terminal>>();
            ViewBag.Products = _appConfig.GetGasProducts().Stringify().Parse<List<Product>>();
            
            return View(new Application());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Application model)
        {
            try
            {
                if (ModelState.IsValid && model.Quantity > 0)
                {
                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    var prevApplication = _application.GetSameQuarterApplication(user.Id, model.QuarterId, model.ProductId);
                    if (prevApplication.Count > 0 && model.ApplicationTypeId != 3)
                    {
                        TempData["Error"] = "Sorry but non-supplementary application for a product type in the sane quarter is not allowed";
                        return RedirectToAction("Index");
                    }
                    var lateApp = LatePayment(model.QuarterId);
                    model.User = user;
                    model.Date = DateTime.UtcNow.AddHours(1);
                    model.LastAssignedUserId = user.Email;
                    var result = _application.Insert(model, lateApp);
                    return RedirectToAction("UploadApplicationDocuments", new {id = result.Id });
                }
                else
                {
                    TempData["Error"] = "An error occured, please try again";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Initiate Application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return RedirectToAction("Index", "DashBoard");
        }
        
        [HttpGet]
        public IActionResult UploadApplicationDocuments(int id)
        {
            ViewData["Message"] = TempData["Message"];
            var docs = new List<DocumentType>();
            try
            {
                docs = _application.GetApplicationFiles(id).Stringify().Parse<List<DocumentType>>();
                ViewBag.companyId = _application.GetCompanyElpsId(User.Identity.Name);
                ViewBag.appId = id;
                ViewData["Status"] = _application.FindById(id)?.Status;
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Display Application Documents",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return View(docs);
        }
        
        [HttpPost]
        public IActionResult UploadApplicationDocuments(int id, PaymentEvidenceViewModel model)
        {
            try
            {
                bool res = false;
                var mailmessage = _elps.GetMailMessages();
                var application = _application.FindById(id);
                if (application.Status.Equals("Rejected"))
                {
                    res = _history.CreateNextProcessingPhase(application, "ResubmitApplication");
                    TempData["Message"] = "Application has been resubmitted successfully";
                    
                    //Notify all staff of submitted application
                    var roles = new[] { "Supervisor", "Officer", "Inspector", "ADGOPS", "HGMR" };
                    var staff = _context.Users.Include(x 
                        => x.UserRoles).ThenInclude(x => x.Role).ToList();
                
                    string subject = "GATEX Application Re-Submission";
                    var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                    var message =
                        $"A {application.ApplicationType.FullName} has been re-submitted for processing. It is currently on {application.LastAssignedUserId}'s desk.";
                    string content = string.Format(body, subject, message, application.Id, DateTime.Now.Year , "https://gatex.dpr.gov.ng");

                    foreach (var user in staff.Where(x => roles.Contains(x.UserRoles.FirstOrDefault().Role.Name)))
                        Utils.SendMail(
                            _emailSettings.Stringify().Parse<Dictionary<string, string>>(), user.Email, subject, content);
                    return RedirectToAction("Index", "Company");
                }               
                else
                {
                    if (Regex.IsMatch(model.ReferenceCode, "\\s\\t[*'\",_&#^@!$%-+/><?{}]"))
                    {
                        TempData["Message"] = "Payment reference contains invalid characters";
                        return RedirectToAction("UploadApplicationDocuments", new {id});
                    }
                    var verify =  
                        _application.ValidatePaymentEvidence(model?.Stringify().Parse<Dictionary<string, string>>());

                    if (application?.PaymentEvidenceId != null)
                    {
                        verify.status = true;
                        verify.hash = application.PaymentEvidence.HashCode;
                        verify.message = "Payment Evidence is valid";
                    }
                    if (verify.status)
                    {
                        //application.LastAssignedUserId = User.Identity.Name;
                        application.PaymentEvidenceId = model.ReferenceType == 1 
                            ? _context.PaymentEvidences.FirstOrDefault(x => x.ReferenceCode.Equals(model.ReferenceCode))?.Id
                            : _context.PaymentEvidences.FirstOrDefault(x => x.HashCode.Equals(model.ReferenceCode))?.Id;
                
                        if (application.Status.Equals(ApplicationStatus.NotSubmitted))
                            res = _history.CreateNextProcessingPhase(application, "SubmitApplication");
                        else if (application.Status.Equals(ApplicationStatus.PaymentNotSatisfied))
                            res = _history.CreateNextProcessingPhase(application, "ResubmitPayment");
                        
                        TempData["Message"] = verify.message;
                    }
                    else
                        TempData["Message"] = verify.message;
                }
                if (res)
                {
                   
                    var message = _message.SendApplicationMessage(application, _hostingEnvironment.WebRootPath, mailmessage,
                        _emailSettings.Stringify().Parse<Dictionary<string, string>>());
                    return RedirectToAction("Payment", new {id = application.Id, reference = application.Reference});
                }
                else
                    TempData["Message"] = "An error occured while submitting this application, please try again or contact support.";
                      
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Upload Application Documents",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return RedirectToAction("UploadApplicationDocuments", new { id });
        }
        
        [HttpGet]
        public IActionResult UpdateAppDoc( int docTypeId, int appid, int docId)
        {
            try
            {
                var result = _application.UpdateApplicationDoc(appid, docTypeId, docId);
                if (result)
                    return Json(data: new { status = true, message = "File update successful" });
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Update Uploaded Application Documents",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return Json(data: new { status = false, message = "File update not successful" });
        }
        
        public IActionResult Payment(int id, int reference)
        {
            var application = _application.FindById(id);
            try
            {
                ViewBag.ApplicationDocs = _application.GetApplicationFiles(id);
                ViewBag.History = _history.GetApplicationHistoriesById(id);
                ViewData["Id"] = id;
                ViewData["AppSubmit"] = $"Your application has been created.\n Please click the button below to submit application to Planning Department.";
                
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Submit Payment",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return View("ApplicationDetail", application);
        }
        
        private bool LatePayment(int quarterid)
        {
            var date = DateTime.Now;
            if (date.Month >= 10 && date.Month <= 12 && date >= new DateTime(date.Year, 9, 1, 00, 00, 00) && quarterid == 4)
                return true;
            if (date.Month >= 7 && date.Month <= 9 && date >= new DateTime(date.Year, 6, 1, 00, 00, 00) && quarterid == 3)
                return true;
            if (date.Month >= 4 && date.Month <= 6 && date >= new DateTime(date.Year, 3, 1, 00, 00, 00) && quarterid == 2)
                return true;
            if (date.Month >= 1 && date.Month <= 3 && date >= new DateTime(date.Year - 1, 12, 1, 00, 00, 00) && quarterid == 1)
                return true;

            return false;
        }
        
        private List<Quarter> LatePaymentText(List<Quarter> quarters)
        {
            var month = DateTime.Now;
            
            if (month.Month >= 1 && month.Month <= 3 && month >= new DateTime(month.Year - 1, 12, 1, 00, 00, 00))
                quarters[0].Name = $"{quarters[0].Name} (Late Application Payment attracts a Fee of Extra $1000.00)";
            if(month.Month >= 4 && month.Month <= 6 && month >= new DateTime(month.Year, 3, 1, 00, 00, 00))
                quarters[1].Name = $"{quarters[1].Name} (Late Application Payment attracts a Fee of Extra $1000.00)";
            if(month.Month >= 7 && month.Month <= 9 && month >= new DateTime(month.Year, 6, 1, 00, 00, 00))
                quarters[2].Name = $"{quarters[2].Name} (Late Application Payment attracts a Fee of Extra $1000.00)>";
            if(month.Month >= 10 && month.Month <= 12 && month >= new DateTime(month.Year, 9, 1, 00, 00, 00))
                quarters[3].Name = $"{quarters[3].Name} (Late Application Payment attracts a Fee of Extra $1000.00)";
            
            return quarters;
        }
        
        public IActionResult CompanyApplication(string email)
        {
            var applications = _application.GetCompanyApplications(email);
            if (applications.Count < 1)
            {
                var comp = _userManager.Users.Include("Company").FirstOrDefault(x => x.Email.Equals(email));
                ViewBag.ComapnyName = comp.Company.Name;
            }
            else
                ViewBag.ComapnyName = applications.FirstOrDefault().User.Company.Name;
            return View(applications);
        } 
        
        public IActionResult All()
        {
            ViewData["Message"] = TempData["Message"];
            return View(_application.GetAll().Stringify().Parse<List<Application>>());
        }
        
        public IActionResult ApplicationDetail(int id)
        {
            var appdocs = _application.GetApplicationFiles(id)?.Stringify().Parse<List<DocumentType>>();
            ViewBag.ApplicationDocs = appdocs.Where(a => a.Selected).ToList();
            ViewBag.History = _history.GetApplicationHistoriesById(id)?.OrderByDescending(x => x.DateAssigned).Take(5);
            return View(_application.FindById(id));
        }
        
        public IActionResult ReadMessage(int id)
        {
            var message = _message.FindById(id);
            if (message != null)
            {
                message.IsRead = true;
                _message.Update(message);
            }
            return Json(data: new { status = true });
        }
        
        public async Task<IActionResult> License(int id, string type = null)
        {
            var permit = _permit.FindById(id);
            
            // if (!string.IsNullOrEmpty(type) && type.Equals("print", StringComparison.OrdinalIgnoreCase))
            // {
            //     var pdf = await new ViewAsPdf("PrintLicense", permit)
            //     {
            //         PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //         FileName = "Approval.pdf"
            //     }.BuildFile(ControllerContext);
            //     return File(new MemoryStream(pdf), "application/pdf");
            // }
            // else
            // {
            //     var pdf = await new ViewAsPdf("License", permit)
            //     {
            //         PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //         FileName = "Approval.pdf"
            //     }.BuildFile(ControllerContext);
            //     return File(new MemoryStream(pdf), "application/pdf");
            // }
            var pdf = await new ViewAsPdf("License", permit)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = "Approval.pdf"
            }.BuildFile(ControllerContext);
            return File(new MemoryStream(pdf), "application/pdf");
        }
        
        public async Task<IActionResult> PreviewLicense(int id)
        {
            var application = _application.FindById(id);
            var permit = new Permit
            {
                Application = application,
                DateIssued = DateTime.UtcNow.AddHours(1),
                ExpiryDate = DateTime.UtcNow.AddHours(1),
                LicenseName = application.ApplicationType.Name,
                PermitNumber = "",
                
            };
            var pdf = await new ViewAsPdf("License", permit)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = "LicensePreview",

            }.BuildFile(ControllerContext);
            return File(new MemoryStream(pdf), "application/pdf");
        }
        
        public IActionResult Report() => View(_application.Report());

        public IActionResult SubmitApplication(int id)
        {
            var application = _application.FindById(id);
            var res = _history.CreateNextProcessingPhase(application, "SubmitPayment");
            if (res)
            {
                TempData["Message"] = "Application has been submitted successfully";
                //Notify all staff of submitted application
                var roles = new[] { "Supervisor", "Officer", "Inspector", "ADGOPS", "HGMR" };
                var staff = _context.Users.Include(x 
                    => x.UserRoles).ThenInclude(x => x.Role).ToList();
                
                string subject = "GATEX Application Submission";
                var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                var message =
                    $"A {application.ApplicationType.FullName} has been submitted for processing. It is currently on {application.LastAssignedUserId}'s desk.";
                string content = string.Format(body, subject, message, application.Id, DateTime.Now.Year , "https://gatex.dpr.gov.ng");

                foreach (var user in staff.Where(x => roles.Contains(x.UserRoles.FirstOrDefault().Role.Name)))
                    Utils.SendMail(
                        _emailSettings.Stringify().Parse<Dictionary<string, string>>(), user.Email, subject, content);
            }            
            else
                TempData["Message"] = "An error occured, please contact ICT/Support";
            
            return RedirectToAction("Index", "Company");
        }
        
        public IActionResult Resubmit(int id)
        {
            var docs = new List<DocumentType>();
            try
            {
                docs = _application.GetApplicationFiles(id).Stringify().Parse<List<DocumentType>>();
                ViewBag.companyId = _application.GetCompanyElpsId(User.Identity.Name);
                ViewBag.appId = id;
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Display Rejected/Resbmit Application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return View(docs);
        }
        
        [HttpPost]
        public IActionResult Resubmit(int id, int companyId)
        {
            try
            {
                bool res = false;
                var application = _application.FindById(id);
                if (application.Status.Equals(ApplicationStatus.PaymentNotSatisfied))
                    res = _history.CreateNextProcessingPhase(application, "SubmitPayment");
                else
                    res = _history.CreateNextProcessingPhase(application, "ResubmitApplication");
                if (res)
                {
                    var mailmessage = _elps.GetMailMessages();
                
                    string subject = "Application Re-Submitted - Ref. No.: " + application.Reference;
                    string tk = string.Format(mailmessage.FirstOrDefault(x => 
                        x.Type.Equals("resubmit", StringComparison.OrdinalIgnoreCase) && x.Category.Equals("company"))?.Content ?? "{0}", application.Reference);
                    TempData["AppSubmitType"] = "Resubmit";
                    var msg = new Message
                    {
                        Content = "Loading...",
                        Date = DateTime.UtcNow.AddHours(1),
                        Subject = subject,
                        User = application.User,
                    };
                    _message.Insert(msg);
                    var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                    var src = "<table class='table table-bordered table-striped'>" +
                              "<tr><td><b>Application Category:</b></td><td>Gas Export Permit</td></tr>" +
                              $"<tr><td><b>Quarter:</b></td><td>{application.Quarter.Name} for " +
                              $"{application.Quantity.ToString("N2")} Barrels of {application.Product.Name}</td></tr></table>";
                    var mailbody = string.Format(body, subject, $"{tk}{src}", msg.Id, DateTime.Now.Year, "");

                    msg.Content = mailbody;
                    _message.Update(msg);
                
                    Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(),
                        User.Identity.Name, subject,mailbody);
                
                    return RedirectToAction("MyDesk", "Company");
                }
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Re-Submit Application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            ViewBag.Error = "An error occured while resubmitting application, please try again";
            return RedirectToAction("Resubmit", new { id = id });
        }
        
        [HttpPost]
        public IActionResult UpdateApplication(int id, int quantity, decimal amount, string gasstream, int product)
        {
            try
            {
                var application = _application.FindById(id);
                if (application != null)
                {
                    application.Quantity = quantity;
                    application.ProductAmount = amount;
                    application.GasStream = gasstream;
                    application.ProductId = product;

                    _application.Update(application);
                    TempData["Message"] = "Application updated successfully";
                }
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Update Application",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
                TempData["Message"] = "An error occured while updating application, please contact support.";
            }

            if (!User.IsInRole("Company"))
                return RedirectToAction("All", "Application");
            
            return RedirectToAction("Index", "Company");
        }

        public async Task<IActionResult> Messages()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            return View(_message.GetListByUserId(user.Id).OrderByDescending(x => x.Date).ToList());
        }

        public IActionResult ViewHistory(int id)
        {
            ViewData["AppId"] = _application.FindById(id).Reference;
            return View(_history.GetApplicationHistoriesById(id)
                .Where(x => x.CurrentUser.Equals(User.Identity.Name, StringComparison.OrdinalIgnoreCase)).ToList());
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var application = _application.FindById(id);
                if (application != null)
                {
                    if(_application.Delete(application))
                        TempData["Message"] = "Application was deleted successfully";
                    else
                        TempData["Message"] = "An error occured while deleting this application, please contact ICT/Support";
                }
            }
            catch (Exception ex)
            {
                
            }

            return RedirectToAction("Index", "Company");
        }
    }
}