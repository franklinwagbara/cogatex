using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
                    var lateApp = LatePayment();
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
                var application = _application.FindById(id);
                var verify =
                    _application.ValidatePaymentEvidence(model.Stringify().Parse<Dictionary<string, string>>());

                if (application?.PaymentEvidenceId != null)
                {
                    verify.status = true;
                    verify.hash = application.PaymentEvidence.HashCode;
                    verify.message = "Payment Evidence is valid";
                }
                if (verify.status)
                {
                    bool res = false;
                    //application.LastAssignedUserId = User.Identity.Name;
                    application.PaymentEvidenceId = model.ReferenceType == 1 
                        ? _context.PaymentEvidences.FirstOrDefault(x => x.ReferenceCode.Equals(model.ReferenceCode))?.Id
                        : _context.PaymentEvidences.FirstOrDefault(x => x.HashCode.Equals(model.ReferenceCode))?.Id;
                    var mailmessage = _elps.GetMailMessages();
            
                    if (application.Status.Equals(ApplicationStatus.NotSubmitted))
                        res = _history.CreateNextProcessingPhase(application, "SubmitApplication");
                    else if (application.Status.Equals(ApplicationStatus.PaymentNotSatisfied))
                        res = _history.CreateNextProcessingPhase(application, "ResubmitPayment");
                    else if (application.Status.Equals(ApplicationStatus.Rejected))
                        res = _history.CreateNextProcessingPhase(application, "ResubmitApplication");

                    if (res)
                    {
                        TempData["Message"] = verify.message;
                        var message = _message.SendApplicationMessage(application, _hostingEnvironment.WebRootPath, mailmessage,
                            _emailSettings.Stringify().Parse<Dictionary<string, string>>());
                        return RedirectToAction("Payment", new {id = application.Id, reference = application.Reference});
                    }
                    else
                        TempData["Message"] = "An error occured while submitting this application, please try again or contact support.";
                }
                else
                    TempData["Message"] = "Payment evidence supplied is invalid, please provide valid payment information.";
                      
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
            return RedirectToAction("UploadApplicationDocuments", new { id = id });
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
                // if (res)
                // {
                    // if(remitapayment.RRR.ToLower() == "DPR-ELPS".ToLower())
                    //     ViewData["AppSubmit"] = "Your application has been re-submitted successfully";
                    // else if(string.Equals(remitapayment.RRR, "DPR-Bank-M", StringComparison.CurrentCultureIgnoreCase))
                    //     ViewData["AppSubmit"] = "Congratulations, your application was submitted successfully";
                    // else
                ViewData["AppSubmit"] = "Your application has been created. Please take the reference number to Planning Department for payment reconciliation";
                
                //Notify all staff of submitted application
                var staff = _userManager.Users.ToList();
                
                string subject = "GATEX Application Submission";
                var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                var message =
                    $"A {application.ApplicationType.FullName} has been submitted for processing. It is currently on {application.LastAssignedUserId}'s desk.";
                string content = string.Format(body, subject, message, application.Id, DateTime.Now.Year);

                foreach (var user in staff)
                {
                    if (_userManager.IsInRoleAsync(user, "Supervisor").Result 
                        || _userManager.IsInRoleAsync(user, "Reviewer").Result 
                        || _userManager.IsInRoleAsync(user, "Officer").Result 
                        || _userManager.IsInRoleAsync(user, "AGGOPS").Result
                        || _userManager.IsInRoleAsync(user, "HGMR").Result)
                        Utils.SendMail(
                            _emailSettings.Stringify().Parse<Dictionary<string, string>>(), user.Email, subject, content);
                }
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
        
        private bool LatePayment()
        {
            var date = DateTime.UtcNow.AddHours(1);
            if (date.Month > 9 && (date.Month == 11 && date.Day > 10) || date.Month > 11)
                return true;
            else if (date.Month > 6 && (date.Month == 8 && date.Day > 10) || date.Month > 8)
                return true;
            else if (date.Month > 3 && (date.Month == 5 && date.Day > 10) || date.Month > 5)
            {
                if(date.Month > 3 && date.Year > 2021)
                    return true;
            }
            else if ((date.Month == 2 && date.Day > 10) || date.Month > 2)
            {
                if(date.Year > 2021)
                    return true;
            }
            return false;
        }
        
        private List<Quarter> LatePaymentText(List<Quarter> quarters)
        {
            var month = DateTime.Now.Month;
            
            if (month >= 1 && month <= 3)
                quarters[0].Name =  $"{quarters[0].Name} (Late Application Payment attracts a Fee of Extra $1000.00)";
            if(month >= 4 && month <= 6)
                quarters[1].Name =  $"{quarters[1].Name} (Late Application Payment attracts a Fee of Extra $1000.00)";
            if(month >= 7 && month <= 9)
                quarters[2].Name =  $"{quarters[2].Name} (Late Application Payment attracts a Fee of Extra $1000.00)>";
            if(month >= 10 && month <= 12)
                quarters[3].Name =  $"{quarters[3].Name} (Late Application Payment attracts a Fee of Extra $1000.00)";
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
        
        public IActionResult All() => View(_application.GetAll().Stringify().Parse<List<Application>>());
        
        public IActionResult ApplicationDetail(int id)
        {
            var appdocs = _application.GetApplicationFiles(id).Stringify().Parse<List<DocumentType>>();
            ViewBag.ApplicationDocs = appdocs.Where(a => a.Selected).ToList();
            ViewBag.History = _history.GetApplicationHistoriesById(id).OrderByDescending(x => x.DateAssigned)
                .Take(5);
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
        
        public IActionResult License(int id, string type = null)
        {
            var permit = _permit.FindById(id);
            //var viewAsPdf = ViewAsPdf("License", permit)
            //{
            //    FileName = "Approval.pdf",
            //    PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            //};
            //if (!string.IsNullOrEmpty(type) && type.Equals("print", StringComparison.OrdinalIgnoreCase))
            //    viewAsPdf.ViewName = "PrintLicense";
            // else
            //     viewAsPdf.ViewName = "License";
            //
            //var pdf = await viewAsPdf.BuildFile(ControllerContext);
            //return File(pdf, "application/pdf");

            if (!string.IsNullOrEmpty(type) && type.Equals("print", StringComparison.OrdinalIgnoreCase))
            {
                
                permit = _permit.All().FirstOrDefault(x => x.ApplicationId == id);
                return new ViewAsPdf("PrintLicense", permit)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    FileName = "Approval.pdf"
                };
            }
             else
            {
                return new ViewAsPdf("License", permit)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    FileName = "Approval.pdf"
                };
            }
        }
        
        public IActionResult PreviewLicense(int id)
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
            //var viewAsPdf = new Rotativa.AspNetCore.ViewAsPdf("License", permit)
            //{
            //    FileName = "Approval.pdf",
            //    PageSize = Rotativa.AspNetCore.Options.Size.A4,
            //    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            //};
            //var pdf = await viewAsPdf.BuildFile(ControllerContext);
            //return File(new MemoryStream(pdf), "application/pdf" );
            return new ViewAsPdf("License", permit)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = "LicensePreview",

            }; ;
        }
        
        public IActionResult Report() => View(_application.Report());

        public IActionResult SubmitApplication(int id)
        {
            var application = _application.FindById(id);
            var res = _history.CreateNextProcessingPhase(application, "SubmitPayment");
            if (res)
                TempData["Message"] = "Application has been submitted successfully";
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
                var application = _application.FindById(id);
                var res = _history.CreateNextProcessingPhase(application, "ResubmitApplication");
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
                    var mailbody = string.Format(body, subject, $"{tk}{src}", msg.Id, DateTime.Now.Year);

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
        public IActionResult UpdateApplication(int id, int quantity, decimal amount, string gasstream)
        {
            try
            {
                var application = _application.FindById(id);
                if (application != null)
                {
                    application.Quantity = quantity;
                    application.ProductAmount = amount;
                    application.GasStream = gasstream;

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