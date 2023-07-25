using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
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
using static System.Net.Mime.MediaTypeNames;
using Application = GOTEX.Core.BusinessObjects.Application;

namespace GOTEX.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly AppDbContext _context;
        private IAppConfiguration<Configuration> _appConfig;
        private readonly IApplication<Application> _application;
        private readonly IRepository<Message> _message;
        private readonly IElpsRepository _elps;
        private readonly IAppHistory<ApplicationHistory> _history;
        private readonly UserManager<ApplicationUser> _userManager;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly IPermit<Permit> _permit;
        private readonly IRepository<Facility> _fac;
        private readonly IRepository<Log> _log;
        private readonly IRepository<DeclarationForm> _dform;
        private readonly EmailSettings _emailSettings;
        private readonly IMapper _mapper;
        
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
            IRepository<Facility> fac,
            IRepository<DeclarationForm> dform,
            IOptionsMonitor<EmailSettings> optionsMonitor,
            IMapper mapper)
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
            _dform = dform;
            _emailSettings = optionsMonitor.CurrentValue;
            _fac = fac;
            _mapper = mapper;
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

            var dform = await VerifyDeclarationForm();

            if (dform)
            {
                quarters = LatePaymentText(quarters);

                ViewBag.Quarters = quarters;
                ViewBag.Terminals = _appConfig.GetTerminal().Stringify().Parse<List<Terminal>>();
                ViewBag.Products = _appConfig.GetGasProducts().Stringify().Parse<List<Product>>();

                return View(new Application());
            }
            return RedirectToAction("AddDelcarationForm");
        }

        private async Task<bool> VerifyDeclarationForm()
        {
            int quarterid = GetQuarterId();
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            return _dform.GetAll().Any(x => x.QuarterId.Equals(quarterid) && x.Year.Equals(DateTime.Now.Year) && x.UserId.Equals(user.Id));
        }

        private int GetQuarterId()
        {
            int quarterid = 0;
            switch (DateTime.Now.Month)
            {
                case 1:
                case 2:
                case 3:
                    quarterid = 1; break;
                case 4:
                case 5:
                case 6:
                    quarterid = 2; break;
                case 7:
                case 8:
                case 9:
                    quarterid = 3; break;
                case 10:
                case 11:
                case 12:
                    quarterid = 4; break;
                default:
                    break;
            }
            return quarterid;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Application model)
        {
            try
            {
                if (ModelState.IsValid && model.Quantity > 0)
                {
                    var user = _userManager.Users.Include(x => x.Company).FirstOrDefault(x => x.Email.Equals(User.Identity.Name));
                    var prevApplication = _application.GetSameQuarterApplication(user.Id, model.QuarterId, model.ProductId);

                    if(prevApplication.Count > 0 && model.QuarterId == 1 && DateTime.UtcNow.Month >= 10)
                        TempData["Success"] = "APplication type is allowed";
                    else if (prevApplication.Count > 0 && model.ApplicationTypeId != 3)
                    {
                        TempData["Error"] = "Sorry but non-supplementary application for a product type in the sane quarter is not allowed";
                        return RedirectToAction("Index");
                    }
                    var lateApp = LatePayment(model.QuarterId);
                    model.User = user;
                    model.Date = DateTime.UtcNow.AddHours(1);
                    model.LastAssignedUserId = user.Email;
                    //check if the terminal has been created as facility for the company
                    var facility = _fac.GetAll().FirstOrDefault(x =>
                        x.CompanyId == user.CompanyId && x.ProductId == model.ProductId &&
                        x.TerminalId == model.TerminalId);

                    if (facility == null)
                    {
                        facility = new Facility
                        {
                            CompanyId = user.CompanyId.GetValueOrDefault(),
                            ProductId = model.ProductId,
                            TerminalId = model.TerminalId,
                            ElpsId = _elps.CreateFacility(new
                            {
                                _context.Terminals.FirstOrDefault(x => x.Id == model.TerminalId).Name,
                                StreetAddress = user.Company.RegisteredAddress,
                                user.CompanyId,
                                DateAdded = DateTime.Now,
                                StateId = 256,
                                FacilityType = "Gas Export Terminal"
                            })
                        };
                        _fac.Insert(facility);
                    }
                    else
                        model.FacilityId = facility.Id;
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

        public IActionResult AddDelcarationForm()
        {
            var model  = new DFormViewModel
            {
                CompanyName = _userManager.Users.Include(c => c.Company).FirstOrDefault(x => x.Email.Equals(User.Identity.Name))?.Company.Name
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddDelcarationForm(DFormViewModel model ) 
        {
            if (model.CrudeTheft != 1 || model.ExportProceedings != 1 || model.OutstandingFee != 1 || model.Offence != 1 || model.Violation != 1)
            {
                TempData["Message"] = "One or more item(s) selection was invalid. Kindly try again.";
                return RedirectToAction("Dashboard", "Company");
            }

            var form = _dform.Insert(new DeclarationForm
            {
                Violation = model.Violation,
                Bribe = model.Bribe,
                OutstandingFee = model.OutstandingFee,
                CrudeTheft = model.CrudeTheft,
                ExportProceedings = model.ExportProceedings,
                ExportVolume = model.ExportVolume,
                Offence = model.Offence,
                QuarterId = GetQuarterId(),
                StaffBribe = model.StaffBribe,
                Year = DateTime.Now.Year,
                UserId = _userManager.Users.FirstOrDefault(x => x.Email.Equals(User.Identity.Name)).Id
            });
            TempData["Message"] = "Declaration form for this quaretr have been saved successfully. You can proceed with your application";
            return RedirectToAction("PrintDeclarationForm", new { form .Id });
        }

        public async Task<IActionResult> PrintDeclarationForm(int id) 
        {
            var dform = _dform.FindById(id);
            var model = _mapper.Map<DFormViewModel>(dform);
            model.PrintType = "print";
            ViewData["DFormPrintType"] = "print";
            model.CompanyName = _userManager.Users.Include(c => c.Company).FirstOrDefault(x => x.Email.Equals(User.Identity.Name))?.Company.Name;
            var pdf = await new ViewAsPdf("AddDelcarationForm", model)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = id + ".pdf"

            }.BuildFile(ControllerContext);
            return File(new MemoryStream(pdf), "application/pdf");
        }

        [HttpGet]
        public IActionResult UploadApplicationDocuments(int id)
        {
            ViewData["Message"] = TempData["Message"];
            var docs = new List<DocumentType>();
            var application = _application.FindById(id);
            try
            {
                var facility = _fac.GetAll().FirstOrDefault(x =>
                    x.CompanyId == application.User.CompanyId && x.ProductId == application.ProductId &&
                    x.TerminalId == application.TerminalId);
                if (facility == null)
                {
                    facility = new Facility
                    {
                        CompanyId = application.User.CompanyId.GetValueOrDefault(),
                        ProductId = application.ProductId,
                        TerminalId = application.TerminalId,
                        ElpsId = _elps.CreateFacility(new
                        {
                            _context.Terminals.FirstOrDefault(x => x.Id == application.TerminalId).Name,
                            StreetAddress = application.User.Company.RegisteredAddress,
                            application.User.CompanyId,
                            DateAdded = DateTime.Now,
                            StateId = 256,
                            FacilityType = "Gas Export Terminal"
                        })
                    };
                    _fac.Insert(facility);
                }
                else
                {
                    application.FacilityId = facility.Id;
                    _application.Update(application);
                }
                docs = _application.GetApplicationFiles(id).Stringify().Parse<List<DocumentType>>();
                ViewBag.companyId = _application.GetCompanyElpsId(User.Identity.Name);
                ViewBag.FacilityId = facility.ElpsId;
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
        public async Task<IActionResult> UploadApplicationDocuments(int id, PaymentEvidenceViewModel model)
        {
            try
            {
                bool res = false;
                var mailmessage = _elps.GetMailMessages();
                var application = _application.FindById(id);
                if (application.Status.Equals("Rejected"))
                {
                    res = await _history.CreateNextProcessingPhase(application, "ResubmitApplication");
                    TempData["Message"] = "Application has been resubmitted successfully";
                    
                    //Notify all staff of submitted application
                    var roles = new[] { "Supervisor", "Officer", "Inspector", "ADGOPS", "HGMR", "ADCOGTO", "ECDP" };
                    var staff = _context.Users.Include(x 
                        => x.UserRoles).ThenInclude(x => x.Role).ToList();
                
                    string subject = "GATEX Application Re-Submission";
                    var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                    var message =
                        $"A {application.ApplicationType.FullName} has been re-submitted for processing. It is currently on {application.LastAssignedUserId}'s desk.";
                    string content = string.Format(body, subject, message, application.Id, DateTime.Now.Year , "https://gatex.nuprc.gov.ng");

                    foreach (var user in staff.Where(x => roles.Contains(x.UserRoles.FirstOrDefault().Role.Name)))
                        Utils.SendMail(
                            _emailSettings.Stringify().Parse<Dictionary<string, string>>(), user.Email, subject, content);
                    return RedirectToAction("Index", "Company");
                }               
                else
                {
                    // if (Regex.IsMatch(model.ReferenceCode, "\\s\\t[*'\",_&#^@!$%-+/><?{}]"))
                    if (Regex.IsMatch(model.ReferenceCode, "[^A-Za-z0-9]"))
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
                            res = await _history.CreateNextProcessingPhase(application, "SubmitPayment");
                        else if (application.Status.Equals(ApplicationStatus.PaymentNotSatisfied))
                            res = await _history.CreateNextProcessingPhase(application, "ResubmitPayment");
                        
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
                quarters[0].Name += " (Late Application Payment attracts a Fee of Extra $1000.00)";
            if(month.Month >= 4 && month.Month <= 6 && month >= new DateTime(month.Year, 3, 1, 00, 00, 00))
                quarters[1].Name += " (Late Application Payment attracts a Fee of Extra $1000.00)";
            if(month.Month >= 7 && month.Month <= 9 && month >= new DateTime(month.Year, 6, 1, 00, 00, 00))
                quarters[2].Name += " (Late Application Payment attracts a Fee of Extra $1000.00)>";
            if(month.Month >= 10 && month.Month <= 12 && month >= new DateTime(month.Year, 9, 1, 00, 00, 00))
                quarters[3].Name += " (Late Application Payment attracts a Fee of Extra $1000.00)";
            
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
            var permit = _application.FindById(id);
            permit.ViewType = type;
            var pdf = await new ViewAsPdf("License", permit?.Permit)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = "Approval.pdf"
            }.BuildFile(ControllerContext);
            ViewData["viewType"] = type;
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

        [HttpPost]
        public async Task<IActionResult> Report(int QuarterId, int Year) 
        {
            var apps = new List<Application>();
            if (QuarterId > 0 && Year > 0)
                apps = _application.Report().Where(x => x.QuarterId == QuarterId && x.Year == Year).ToList();
            else if (QuarterId > 0 && Year == 0)
                apps = _application.Report().Where(x => x.QuarterId == QuarterId).ToList();
            else if (QuarterId == 0 && Year > 0)
                apps = _application.Report().Where(x => x.Year == Year).ToList();
            else
                apps = _application.Report();
            return View(apps);
        }

        public async Task<IActionResult> SubmitApplication(int id)
        {
            var application = _application.FindById(id);
            var res = await _history.CreateNextProcessingPhase(application, "SubmitPayment");
            if (res)
            {
                TempData["Message"] = "Application has been submitted successfully";
                //Notify all staff of submitted application
                var roles = new[] { "Supervisor", "Officer", "Inspector", "ADGOPS", "HGMR", "Planning", "ECDP", "ADCOGTO" };
                var staff = _context.Users.Include(x 
                    => x.UserRoles).ThenInclude(x => x.Role).ToList();

                var planning = _userManager.Users.Include(x => x.UserRoles).FirstOrDefault(x => x.Email.Equals(application.LastAssignedUserId));

                if (planning != null)
                    staff.Add(planning);
                
                string subject = "GATEX Application Submission";
                var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                var message =
                    $"A {application.ApplicationType.FullName} has been submitted for processing. It is currently on {application.LastAssignedUserId}'s desk.";
                string content = string.Format(body, subject, message, application.Id, DateTime.Now.Year , "https://gatex.nuprc.gov.ng");

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
            var application = _application.FindById(id);
            try
            {
                var facility = _fac.GetAll().FirstOrDefault(x =>
                    x.CompanyId == application.User.CompanyId && x.ProductId == application.ProductId &&
                    x.TerminalId == application.TerminalId);
                if (facility == null)
                {
                    facility = new Facility
                    {
                        CompanyId = application.User.CompanyId.GetValueOrDefault(),
                        ProductId = application.ProductId,
                        TerminalId = application.TerminalId,
                        ElpsId = _elps.CreateFacility(new
                        {
                            _context.Terminals.FirstOrDefault(x => x.Id == application.TerminalId).Name,
                            StreetAddress = application.User.Company.RegisteredAddress,
                            application.User.CompanyId,
                            DateAdded = DateTime.Now,
                            StateId = 256,
                            FacilityType = "Gas Export Terminal"
                        })
                    };
                    _fac.Insert(facility);
                }
                else
                {
                    application.FacilityId = facility.Id;
                    _application.Update(application);
                }
                docs = _application.GetApplicationFiles(id).Stringify().Parse<List<DocumentType>>();
                ViewBag.companyId = _application.GetCompanyElpsId(User.Identity.Name);
                ViewBag.FacilityId = facility.ElpsId;
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
        public async Task<IActionResult> Resubmit(int id, int companyId)
        {
            try
            {
                bool res = false;
                var application = _application.FindById(id);
                if (application.Status.Equals(ApplicationStatus.PaymentNotSatisfied))
                {
                    res = await _history.CreateNextProcessingPhase(application, "SubmitPayment");                    
                }

                else
                    res = await _history.CreateNextProcessingPhase(application, "ResubmitApplication");
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

                    if(application.Status.ToLower().Equals("payment pending"))
                    {
                        var planning = _userManager.Users.Include(x => x.UserRoles).FirstOrDefault(x => x.Email.Equals(application.LastAssignedUserId));
                        Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(),
                       planning.Email, subject, mailbody);
                    }

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

        public IActionResult UpdateApplication(int id)
        { 
            var app = _application.FindById(id);
            return View(new ApplicationViewModel 
            { 
                Amount = app.ProductAmount,
                ApplicationTypeId = app.ApplicationTypeId,
                GasStream = app.GasStream,
                Id = app.Id,
                LastAssignedUserId = app.LastAssignedUserId,
                ProductId = app.ProductId,
                Quantity = app.Quantity,
                QuarterId = app.QuarterId,
                StageId = app.StageId,
                Status = app.Status,
                TerminalId = app.TerminalId,
                ApplicationType = _appConfig.GetApplicationTypes().Stringify().Parse<List<ApplicationType>>(),
                Quarter = _appConfig.GetQuarters().Stringify().Parse<List<Quarter>>(),
                Product = _appConfig.GetGasProducts().Stringify().Parse<List<Product>>(),
                Terminal = _appConfig.GetTerminal().Stringify().Parse<List<Terminal>>(),
            });
        }

        [HttpPost]
        public IActionResult UpdateApplication(ApplicationViewModel model)
        {
            try
            {
                var application = _application.FindById(model.Id);
                if (application != null)
                {
                    application.Quantity = model.Quantity;
                    application.ProductAmount = model.Amount;
                    application.GasStream = model.GasStream;
                    application.ProductId = model.ProductId;
                    application.TerminalId = model.TerminalId;
                    application.FacilityId = model.TerminalId;
                    if (User.IsInRole("Admin"))
                    {
                        application.StageId = model.StageId;
                        application.LastAssignedUserId = model.LastAssignedUserId;
                        application.Status = model.Status;
                    }
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