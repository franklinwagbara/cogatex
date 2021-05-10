using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace GOTEX.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IApplication<Application> _application;
        private readonly IAppHistory<ApplicationHistory> _history;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPayment<PaymentApproval> _paymentApproval;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IAppConfiguration<Configuration> _appConfig;
        private readonly IElpsRepository _elps;
        private readonly IRepository<Message> _message;
        private readonly IPermit<Permit> _permit;
        private readonly EmailSettings _emailSettings;
        private readonly IRepository<Log> _log;
        private readonly IApplicationTypeDocs<ApplicationTypeDocuments> _applicationTypeDocs;
        private readonly IRepository<ApplicationType> _appTypes;

        public AdminController(
            IApplication<Application> application,
            IAppHistory<ApplicationHistory> history,
            UserManager<ApplicationUser> userManager,
            IPayment<PaymentApproval> paymentApproval,
            IWebHostEnvironment hostingEnvironment,
            IAppConfiguration<Configuration> appConfig,
            IElpsRepository elps,
            IRepository<Message> message,
            IPermit<Permit> permit,
            IOptionsMonitor<EmailSettings> optionsMonitor,
            IRepository<Log> log,
            IApplicationTypeDocs<ApplicationTypeDocuments> applicationTypeDocs,
            IRepository<ApplicationType> appTypes)
        {
            _application = application;
            _history = history;
            _userManager = userManager;
            _paymentApproval = paymentApproval;
            _hostingEnvironment = hostingEnvironment;
            _appConfig = appConfig;
            _elps = elps;
            _message = message;
            _permit = permit;
            _emailSettings = optionsMonitor.CurrentValue;
            _log = log;
            _applicationTypeDocs = applicationTypeDocs;
            _appTypes = appTypes;
        }

        public IActionResult Index()
        {
            ViewBag.Message = TempData["message"];
            if (User.IsInRole("Company"))
                return RedirectToAction("MyDesk", "Company");

            return View();
        }

        public IActionResult MyDesk()
        {
            IEnumerable<Application> allApplications = _application.GetAll();

            allApplications = allApplications.Where(a => a.LastAssignedUserId == User.Identity.Name); //&& a.Submitted

            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();

                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                var sortColumn = Request
                    .Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;

                int skip = start != null ? Convert.ToInt32(start) : 0;

                int recordsTotal = 0;

                // getting all Customer data  
                // var customerData = (from tempcustomer in _context.CustomerTB  
                //                     select tempcustomer);  
                //Sorting  
                // if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))  
                // {  
                //     allApplications = allApplications.OrderBy(sortColumn + " " + sortColumnDirection);  
                // }  
                //Search  
                if (!string.IsNullOrEmpty(searchValue))
                {
                    allApplications = allApplications.Where(m => m.Reference == searchValue
                                                                 || m.User.Company.Name.StartsWith(searchValue));
                }

                //total number of rows counts   
                recordsTotal = allApplications.Count();
                //Paging   
                var data = allApplications.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new
                    {draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data});
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "AGet APplications on my desk",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }

            return Json(new {draw = "", recordsFiltered = 0, recordsTotal = 0, data = ""});
        }

        public IActionResult ViewApplication(int id)
        {
            var application = _application.FindById(id);
            try
            {
                if (application == null)
                {
                    TempData["message"] = "Invalid application credentials";
                    return RedirectToAction("Index");
                }

                ViewBag.ErrorMessage = TempData["AppSubmitType"];

                var appdocs = _application.GetApplicationFiles(id).Stringify().Parse<List<DocumentType>>();
                ViewBag.ApplicationDocs = appdocs.Where(a => a.Selected).ToList();
                //ViewBag.DocsRemaining = appdocs.Where(a => a.Selected == false).Count();
                ViewBag.History = _history.GetApplicationHistoriesById(id).OrderByDescending(x => x.DateAssigned)
                    .Take(2);
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "View Application Details",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }

            return View(application);
        }

        [HttpPost]
        [Authorize(Roles = ("Planning"))]
        public async Task<IActionResult> ConfirmPayment(ConfirmPaymentViewModel model)
        {
            var application = _application.FindByReference(model.refno);
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            try
            {
                if (application == null)
                    TempData["Message"] = "Application with reference number " + model.refno +
                                          " is invalid. Please check and try again.";
                else
                {
                    var payment = new PaymentApproval
                    {
                        Amount = (decimal) model.amount,
                        ApplicationId = application.Id,
                        Bank = model.bank,
                        Comment = model.feedback,
                        Date = DateTime.UtcNow.AddHours(1),
                        PaymentId = model.bankPayId,
                        PaymentUrl = "",
                        Status = model.submitBtn,
                        User = await _userManager.FindByEmailAsync(User.Identity.Name),
                        ApprovedBy = $"{user.LastName} {user.LastName} ({User.Identity.Name})"
                    };
                    _paymentApproval.Insert(payment);

                    if (model.submitBtn == "Payment Not Satisfied")
                    {
                        application.Status = ApplicationStatus.PaymentNotSatisfied;
                        application.Submitted = false;
                        application.LastAssignedUserId = application.User.Email;

                        _application.Update(application);

                        //Reject application to client/marketer
                        _history.CreateNextProcessingPhase(application, "RejectPayment", model.feedback);
                        //Send client/marketer a notification

                        _application.InvalidatePaymentonElps(
                            _hostingEnvironment.WebRootPath,
                            application,
                            model.feedback,
                            _emailSettings.Stringify().Parse<Dictionary<string, string>>());
                        TempData["message"] = "Application has been returned to the client for proper review";
                        return RedirectToAction("Index");
                    }

                    var manualpayment = new ManualRemitaValue
                    {
                        RRR = application.Reference,
                        AddedBy = application.LastAssignedUserId,
                        Beneficiary = "DPR",
                        DateAdded = DateTime.UtcNow.AddHours(1),
                        FundingBank = "",
                        NetAmount = Convert.ToDouble(application.Fee + application.ServiceCharge),
                        PaymentSource = "Bank",
                        Status = ""
                    };
                    var description =
                        _paymentApproval.CreateManualPayment(application, manualpayment, model.amount, model.feedback);

                    application.Description = description;
                    application.Status = ApplicationStatus.Processing;
                    application.Submitted = true;
                    _application.Update(application);
                    
                    var mailtype = $"{application.ApplicationType.Name} {application.Quarter.Name}";

                    var res = _history.CreateNextProcessingPhase(application, "ConfirmPayment", model.feedback);
                    if (res)
                    {
                        var message = new Message
                        {
                            Date = DateTime.UtcNow.AddHours(1),
                            Subject = $"Application for {mailtype}",
                            ApplicationId = application.Id,
                            User = _userManager.FindByEmailAsync(application.LastAssignedUserId).Result
                        };
                        _message.Insert(message);
                        var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                        
                        TempData["message"] =
                            "Payment confirmation successful and application has been sent to the next processing staff.";
                        SendMail(application, message.Content, message.Subject, body, "Accept", mailtype, message);
                    }
                    else
                        TempData["message"] =
                            "An error occured, please contact Suppport/ICT";

                }
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Payment Confirmation ",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CompanyList()
        {
            ViewData["Message"] = TempData["Message"];
            var companies = _userManager.Users
                .Include(x => x.Company).Where(y => y.CompanyId != null).ToList();

            return View(companies);
        }

        public IActionResult Approve(ApprovalViewModel model)
        {
            try
            {
                var application = _application.FindById(model.APplicationId);

                _history.CreateNextProcessingPhase(application, model.Action, model.Report);
                
                var mailtype = $"{application.ApplicationType.Name} {application.Quarter.Name}";
                var message = new Message
                {
                    Date = DateTime.UtcNow.AddHours(1),
                    Subject = $"Application for {mailtype}",
                    ApplicationId = application.Id,
                    User = _userManager.FindByEmailAsync(application.LastAssignedUserId).Result
                };
                _message.Insert(message);
                var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");

                if (User.IsInRole("OOD") && model.Action.Contains("Approve"))
                { 
                    var tk = $"Application for {mailtype} with reference: {application.Reference} on DPR Gas Export Permit portal (GATEX) has been approved: " +
                           $"<br /> {model.Report}. <br/> PLease await further actions concerning your approved Application Form.";
                    message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, $"https://gatex.dpr..gov.ng/account/login?email={application.LastAssignedUserId}");
                    
                    application = _application.FindById(model.APplicationId);
                    if (application.Status.Equals(ApplicationStatus.Completed))
                    {
                        var permitnumber = _permit.CreatePermit(application.Id,
                            _hostingEnvironment.WebRootPath,
                            _emailSettings.Stringify().Parse<Dictionary<string, string>>());
                        TempData["message"] = "You have APPROVED this Application (" + application.Reference +
                                              ")  and Permit has been recommended for issuance. Permit No: " + permitnumber;
                    }
                    
                    _message.Update(message);
                    Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(),
                        application.LastAssignedUserId, 
                        message.Subject, message.Content);
                }
                else
                    SendMail(application, message.Content, message.Subject, body, model.Action, mailtype, message);

            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Approval/Decline action ",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }

            return RedirectToAction("Index");
        }

        public IActionResult Permits() => View(_permit.All());

        public IActionResult ApplicationReport() => View();

        [HttpPost]
        public IActionResult ApplicationReport(string startDate, string endDate)
        {
            var chart = new List<ChartModel>();
            DateTime start = string.IsNullOrEmpty(startDate)
                ? DateTime.Today.AddDays(-29).Date
                : DateTime.Parse(startDate);
            DateTime end = string.IsNullOrEmpty(endDate)
                ? DateTime.Today.AddDays(-29).Date
                : DateTime.Parse(endDate);
            var applications = _application.GetAll()
                .Where(x => x.Submitted && x.Date >= start && x.Date <= end)
                .OrderByDescending(y => y.Date)
                .ToList();

            chart = applications.GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day)).Select( y => new ChartModel
            {
                Date = y.Key.Date.ToShortDateString(),
                Approved = y.Count(z => z.Status.Equals("Completed")),
                Processing = y.Count(z => z.Status.Equals("Processing")),
                Rejected = y.Count(x => x.Status.Equals("Rejected"))
            }).ToList();
            return View(chart);
        }

        public IActionResult PaymentReport()
        {
            var payments = GetPayments();
            var stdate = DateTime.UtcNow.AddDays(-29);
            var edate = DateTime.UtcNow.AddHours(1);

            payments = payments.Where(x => x.Date >= stdate.Date && x.Date <= edate).ToList();
            
            return View(payments);
        }

        [HttpPost]
        public IActionResult PaymentReport(PaymentReportViewModel model)
        {
            var payments = GetPayments();
            var stdate = string.IsNullOrEmpty(model.startdate) ? DateTime.UtcNow.AddDays(-29) : Convert.ToDateTime(model.startdate).Date;
            var edate = string.IsNullOrEmpty(model.enddate) ? DateTime.UtcNow : Convert.ToDateTime(model.enddate).Date.AddHours(23).AddMinutes(59);;
            
            payments = payments.Where(x => x.Date >= stdate.Date && x.Date <= edate).ToList();
            
            return View(payments);

        }
        
        private List<PaymentApproval> GetPayments() => _paymentApproval.GetAll();

        public IActionResult FilterPermit()
        {
            throw new NotImplementedException();
        }

        public IActionResult ApplicationTypes()
        {
            ViewData["Message"] = TempData["Message"];
            ViewData["alert"] = TempData["alert"];
            return View(_appTypes.GetAll());
        }

        public IActionResult EditApplicationType(ApplicationType model)
        {
            if (ModelState.IsValid)
            {
                _appTypes.Update(model);
                TempData["Message"] = "Update was successful";
                TempData["alert"] = "alert-success";
            }
            else
            {
                TempData["Message"] = "Update was not successful";
                TempData["alert"] = "alert-warning";
            }

            return RedirectToAction("ApplicationTypes");
        }

        public IActionResult ManageDocuments(int id)
        {
            var compdocs = new List<DocumentType>();
            var facdocs = new List<DocumentType>();
            var systemdocs = _applicationTypeDocs.GetAll().Where(x => x.ApplicationTypeId == id).ToList();
            var companydocs = _elps.GetDocumentTypes().Stringify().Parse<List<DocumentType>>();
            var facilitydocs = _elps.GetDocumentTypes("facility").Stringify().Parse<List<DocumentType>>();
            foreach (var doc in companydocs)
            {
                if (systemdocs.Any(x => x.DocumentTypeId == doc.Id))
                {
                    doc.Selected = true;
                    compdocs.Add(doc);
                }
                else
                {
                    doc.Selected = false;
                    compdocs.Add(doc);
                }
            }
            foreach (var doc in facilitydocs)
            {
                if (systemdocs.Any(x => x.DocumentTypeId == doc.Id))
                {
                    doc.Selected = true;
                    facdocs.Add(doc);
                }
                else
                {
                    doc.Selected = false;
                    facdocs.Add(doc);
                }
            }
            
            return Json(new {compdocs, facdocs});
        }

        [HttpPost]
        public IActionResult UpdateApplicationTypeDocuments(DocumentUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var apptypedocs = _applicationTypeDocs.GetAll()
                    .Where(x => x.ApplicationTypeId == model.ApplicationTypeId).ToList();
                if (apptypedocs != null)
                {
                    foreach (var doc in model.DocId)
                    {
                        if(apptypedocs.Any(x => x.DocumentTypeId == doc))
                            continue;
                        else
                        {
                            _applicationTypeDocs.InsertDocs(new ApplicationTypeDocuments
                            {
                                 ApplicationTypeId = model.ApplicationTypeId,
                                 DocumentTypeId = doc
                            });
                        }
                    }

                    foreach (var sysdocs in apptypedocs)
                    {
                        if (!model.DocId.Contains(sysdocs.DocumentTypeId))
                            _applicationTypeDocs.Delete(sysdocs);
                    }
                }
                
            }

            return RedirectToAction("ApplicationTypes");
        }

        public IActionResult StaffDesk() => View(_userManager.Users.ToList());

        public IActionResult Outbox()
        {
            var history = _history.SentApplications(User.Identity.Name);
            var apps = history
                .GroupBy(x => x.ApplicationId)
                .Select(x => x.FirstOrDefault())
                .OrderByDescending(x => x.DateAssigned)
                .ThenByDescending(x => x.DateAssigned)
                .Select(x => new SentItems
                {
                    DateApplied = x.Application.Date,
                    CompanyName = x.Application.User.Company.Name,
                    Reference = x.Application.Reference,
                    AppType = x.Application.ApplicationType.Name,
                    DateTreated = x.DateAssigned,
                    Product = x.Application.Product.Name,
                    Terminal = x.Application.Terminal.Name,
                    Quarter = x.Application.Quarter.Name,
                    Action = x.Action,
                    Comment = x.Comment
                }).ToList();
            return View(apps);
        }

        private void SendMail(Application application, string comment, string subject, string body, string action, string mailtype, Message message)
        {
            var tk = $"Application for {mailtype} with reference: {application.Reference} on <br/>DPR Gas Export Permit portal (GATEX) has been sent to you for further action: " +
                     $"<br /> {comment}. <br/>";
            message.Content = string.Format(body, message.Subject, tk, message.Id);
                   
            if(action.ToLower().Equals("accept"))
                TempData["message"] =                                                                    
                    "Application has been pushed to the next processing officer for appropriate action";
            else if(action.ToLower().Equals("reject"))
            {
                TempData["message"] =
                    "Application has been rejected to the immediate processing officer for appropriate action";
                if (application.LastAssignedUserId.Equals(application.UserId))
                    TempData["message"] =
                        "Application has been rejected to the marketer for appropriate action";
            }
            _message.Update(message);
            Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(),
                application.LastAssignedUserId, 
                message.Subject, message.Content);
        }
    }
}