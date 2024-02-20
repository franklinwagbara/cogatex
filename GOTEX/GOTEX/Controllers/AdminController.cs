using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly IRepository<WorkFlow> _workFlow;
        private readonly IRepository<PaymentEvidence> _paymentEvidence;
        private readonly IRepository<Product> _product;
        private readonly IRepository<Leave> _leaveRepo;


        public AdminController(
            IApplication<Application> application,
            IAppHistory<ApplicationHistory> history,
            UserManager<ApplicationUser> userManager,
            IPayment<PaymentApproval> paymentApproval,
            IWebHostEnvironment hostingEnvironment,
            IAppConfiguration<Configuration> appConfig,
            IElpsRepository elps,
            IRepository<Message> message,
            IRepository<Product> product,
            IPermit<Permit> permit,
            IOptionsMonitor<EmailSettings> optionsMonitor,
            IRepository<Log> log,
            IApplicationTypeDocs<ApplicationTypeDocuments> applicationTypeDocs,
            IRepository<ApplicationType> appTypes,
            IRepository<WorkFlow> workFlow,
            IRepository<Leave> leaveRepo,
            IRepository<PaymentEvidence> paymentEvidence)
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
            _workFlow = workFlow;
            _paymentEvidence = paymentEvidence;
            _product = product;
            _leaveRepo = leaveRepo;
        }

        public IActionResult Index()
        {
            ViewBag.Message = TempData["message"];
            if (User.IsInRole("Company"))
                return RedirectToAction("MyDesk", "Company");

            return View();
        }

        public async Task<IActionResult> MyDesk()
        {
            try
            {
                IEnumerable<Application> allApplications = _application.GetAll();

                var appOnMyDesk = allApplications.Where(a => a.LastAssignedUserId == User.Identity.Name); //&& a.Submitted

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

                if (!string.IsNullOrEmpty(searchValue))
                {
                    appOnMyDesk = appOnMyDesk.Where(m => m.Reference == searchValue
                                                                 || m.User.Company.Name.StartsWith(searchValue));
                }

                //total number of rows counts   
                recordsTotal = appOnMyDesk.Count();
                //Paging   
                var data = appOnMyDesk.Skip(skip).Take(pageSize).ToList();
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

        public async Task<IActionResult> LeaverDesk()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var leaveEntry = _leaveRepo.GetAll().Where(x => x.ActingStaffId == user.Id).FirstOrDefault();

                IEnumerable<Application> allApplications = _application.GetAll();

                IEnumerable<Application> appOnLeaverDesk = new List<Application>();
                ApplicationUser userOnLeave = null;

                if (leaveEntry != null)
                {
                    userOnLeave = await _userManager.FindByIdAsync(leaveEntry.StaffId);
                    appOnLeaverDesk = allApplications.Where(x => x.LastAssignedUserId == userOnLeave.Email);
                }

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
 
                if (!string.IsNullOrEmpty(searchValue))
                {
                    appOnLeaverDesk = appOnLeaverDesk.Where(m => m.Reference == searchValue
                                                                 || m.User.Company.Name.StartsWith(searchValue));
                }

                //total number of rows counts   
                recordsTotal = appOnLeaverDesk.Count();
                //Paging   
                var data = appOnLeaverDesk.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data  
                return Json(new
                { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Get Applications on staff's on leave desk",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }

            return Json(new { draw = "", recordsFiltered = 0, recordsTotal = 0, data = "" });
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaverInfo()
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var leaveEntry = _leaveRepo.GetAll().Where(x => x.ActingStaffId == user.Id).FirstOrDefault();
                ApplicationUser userOnLeave = null;

                if (leaveEntry != null)
                    userOnLeave = await _userManager.FindByIdAsync(leaveEntry.StaffId);

                return Json(new { data = userOnLeave });
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Get Applications on staff's on leave desk",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }

            return Json(new { data = "" });
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
                        ApprovedBy = $"{user.FirstName} {user.LastName} ({User.Identity.Name})"
                    };
                    _paymentApproval.Insert(payment);

                    if (model.submitBtn == "Payment Not Satisfied")
                    {
                        application.Status = ApplicationStatus.PaymentNotSatisfied;
                        application.Submitted = false;
                        application.LastAssignedUserId = application.User.Email;

                        _application.Update(application);

                        //Reject application to client/marketer
                        await _history.CreateNextProcessingPhase(application, "RejectPayment", model.feedback);
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
                        Beneficiary = "NUPRC",
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

                    var res = await _history.CreateNextProcessingPhase(application, "ConfirmPayment", model.feedback);
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

        public async Task<IActionResult> Approve(ApprovalViewModel model)
        {
            try
            {
                var application = _application.FindById(model.APplicationId);
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                var leave = _leaveRepo.GetAll().FirstOrDefault(x => x.ActingStaffId == user.Id && x.IsApproved == true && x.IsDeleted == false);
                ApplicationUser leaver = null;

                if (leave != null)
                    leaver = await _userManager.Users.Include(x => x.UserRoles).Where(x => x.Id == leave.StaffId).FirstOrDefaultAsync();

                if (User.Identity.Name.Equals(application.LastAssignedUserId))
                {
                    if (User.IsInRole(Roles.CCE))
                        model.Report = model.Action.ToLower().Equals("approve") ? "Final approval by CCE" : "Application rejected by CCE";

                    var res = await _history.CreateNextProcessingPhase(application, model.Action, model.Report);
                    
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

                    if ((User.IsInRole("CCE") || User.IsInRole("OOCCE")) && model.Action.Contains("Approve"))
                    { 
                        var tk = $"Application for {mailtype} with reference: {application.Reference} on NUPRC Gas Export Permit portal (GATEX) has been approved: " +
                               $"<br /> {model.Report}. <br/> PLease await further actions concerning your approved Application Form.";
                        message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, $"https://gatex.nuprc.gov.ng/account/login?email={application.LastAssignedUserId}");
                        message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, $"https://gatex.nuprc.gov.ng/account/login?email={application.LastAssignedUserId}");
                        
                        application = _application.FindById(model.APplicationId);
                        if (application.Status.Equals(ApplicationStatus.Completed))
                        {
                            var permitnumber = _permit.CreatePermit(application.Id,
                                _hostingEnvironment.WebRootPath,
                                _emailSettings.Stringify().Parse<Dictionary<string, string>>());
                            TempData["message"] = "You have APPROVED this Application (" + application.Reference +
                                                  ")  and Approval has been issued with Approval No: " + permitnumber;
                        }                        
                        _message.Update(message);
                        Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(), application.LastAssignedUserId, message.Subject, message.Content);
                    }
                    else
                    {
                        SendMail(application, model.Report, message.Subject, body, model.Action, mailtype, message);
                        if(model.Action.ToLower().Equals("approve"))
                            TempData["message"] =                                                                    
                                "Application has been pushed to the next processing officer for appropriate action";
                        else if(model.Action.ToLower().Equals("reject"))
                        {
                            TempData["message"] =
                                "Application has been rejected to the immediate processing officer for appropriate action";
                            if (application.LastAssignedUserId.Equals(application.UserId))
                                TempData["message"] =
                                    "Application has been rejected to the marketer for appropriate action";
                        }
                    }
                }
                else if (leaver != null && leaver.Email.Equals(application.LastAssignedUserId))
                {
                    if(_userManager.IsInRoleAsync(leaver, Roles.CCE).Result)
                        model.Report = model.Action.ToLower().Equals("approve") ? "Final approval by CCE" : "Application rejected by CCE";

                    var res = await _history.CreateNextProcessingPhase(application, model.Action, model.Report);
                    
                    var mailtype = $"{application.ApplicationType.Name} {application.Quarter.Name}";
                    var message = new Message
                    {
                        Date = DateTime.UtcNow.AddHours(1),
                        Subject = $"Application for {mailtype}",
                        ApplicationId = application.Id,
                        User = _userManager.FindByEmailAsync(application.LastAssignedUserId).Result
                    };
                    _message.Insert(message);

                    var messageToActingStaff = new Message
                    {
                        Date = DateTime.UtcNow.AddHours(1),
                        Subject = $"Application for {mailtype}",
                        ApplicationId = application.Id,
                        User = await _userManager.FindByIdAsync(leave.ActingStaffId)
                    };
                    _message.Insert(messageToActingStaff);

                    var body = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");

                    if (_userManager.IsInRoleAsync(leaver, Roles.CCE).Result && model.Action.Contains("Approve"))
                    { 
                        var tk = $"Application for {mailtype} with reference: {application.Reference} on NUPRC Gas Export Permit portal (GATEX) has been approved: " +
                               $"<br /> {model.Report}. <br/> PLease await further actions concerning your approved Application Form.";
                        message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, $"https://gatex.nuprc.gov.ng/account/login?email={application.LastAssignedUserId}");
                        messageToActingStaff.Content = string.Format(body, messageToActingStaff.Subject, tk, messageToActingStaff.Id, DateTime.Now.Year, $"https://gatex.nuprc.gov.ng/account/login?email={User.Identity.Name}");

                        application = _application.FindById(model.APplicationId);
                        if (application.Status.Equals(ApplicationStatus.Completed))
                        {
                            var permitnumber = _permit.CreatePermit(application.Id,
                                _hostingEnvironment.WebRootPath,
                                _emailSettings.Stringify().Parse<Dictionary<string, string>>());
                            TempData["message"] = "You have APPROVED this Application (" + application.Reference +
                                                  ")  and Approval has been issued with Approval No: " + permitnumber;
                        }                        
                        _message.Update(message);
                        _message.Update(messageToActingStaff);
                        Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(), application.LastAssignedUserId, message.Subject, message.Content);
                        Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(), User.Identity.Name, messageToActingStaff.Subject, messageToActingStaff.Content);
                    }
                    else
                    {
                        SendMail(application, model.Report, message.Subject, body, model.Action, mailtype, message);
                        SendMail(application, model.Report, messageToActingStaff.Subject, body, model.Action, mailtype, messageToActingStaff, User.Identity.Name);

                        if (model.Action.ToLower().Equals("approve"))
                            TempData["message"] =                                                                    
                                "Application has been pushed to the next processing officer for appropriate action";
                        else if(model.Action.ToLower().Equals("reject"))
                        {
                            TempData["message"] =
                                "Application has been rejected to the immediate processing officer for appropriate action";
                            if (application.LastAssignedUserId.Equals(application.UserId))
                                TempData["message"] =
                                    "Application has been rejected to the marketer for appropriate action";
                        }
                    }
                }
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

        public IActionResult RecreatePermitNo(int id)
        {
            var permit = _permit.FindById(id);
            if (permit != null)
            {
                var str = permit.PermitNumber.Split('/').ToList();
                str.RemoveAt(0);
                permit.PermitNumber = $"NUPRC/{String.Join("/", str)}";
                permit.ElpsId = _elps.PushPermitToElps(new PermitAPIModel
                {
                    CategoryName = "Gas Export Permit",
                    Company_Id = permit.Application.User.Company.ElpsId,
                    Date_Expire = permit.ExpiryDate,
                    Date_Issued = permit.DateIssued,
                    Permit_No = permit.PermitNumber,
                    Is_Renewed = permit.IsRenewed,
                });
                _permit.Update(permit);
            }

            return RedirectToAction("Permits");
        }

        public IActionResult ApplicationReport()
        {
            var chart = new List<ChartModel>();
            var applications = AppReport();

            chart = applications.GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day)).Select(y => new ChartModel
            {
                Date = y.Key.Date.ToShortDateString(),
                Approved = y.Count(z => z.Status.Equals("Completed")),
                Processing = y.Count(z => z.Status.Equals("Processing")),
                Rejected = y.Count(x => x.Status.Equals("Rejected"))
            }).ToList();

            ViewData["Chart"] = chart;

            return View(applications);
        }

        private List<Application> AppReport(string startDate = null, string endDate = null, string status = null)
        {
            DateTime start = string.IsNullOrEmpty(startDate)
                ? DateTime.Today.AddDays(-29).Date
                : DateTime.ParseExact(startDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime end = string.IsNullOrEmpty(endDate)
                ? DateTime.Today.AddDays(-29).Date
                : DateTime.ParseExact(endDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            var apps = _application.GetAll()
                .Where(x => x.Date >= start && x.Date <= end)
                .OrderByDescending(y => y.Date)
                .ToList();
            if(!string.IsNullOrEmpty(status) && apps.Count > 0)
                apps = apps.Where(x => x.Status.ToLower().Contains(status.ToLower())).ToList();
            return apps;
        }

        [HttpPost]
        public IActionResult ApplicationReport(string startDate, string endDate, string status)
        {
            var chart = new List<ChartModel>();
            var applications = AppReport(startDate, endDate, status);

            chart = applications.GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, x.Date.Day)).Select( y => new ChartModel
            {
                Date = y.Key.Date.ToShortDateString(),
                Approved = y.Count(z => z.Status.Equals("Completed")),
                Processing = y.Count(z => z.Status.Equals("Processing")),
                Rejected = y.Count(x => x.Status.Equals("Rejected"))
            }).ToList();

            ViewData["Chart"] = chart;

            return View(applications);
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

        [HttpPost]
        public IActionResult FilterPermit(DateRangeFilterViewModel Model)
        {
            try
            {
                var permits = _permit.All();
                if(ModelState.IsValid)
                {
                    permits = permits.Where(x => x.DateIssued <= Model.End && x.DateIssued >= Model.Start).ToList();
                    return View("Permits", permits);
                }
                else
                {
                    TempData["Error"] = "You must select both start and end datetime to filter.";
                    return RedirectToAction("Permits");
                }
            }
            catch (Exception ex)
            {
                _log.Insert(new Log
                {
                    Action = "Filter Approved Applications.",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = User.Identity.Name
                });
            }
            return RedirectToAction("Permits");
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
                
                apptypedocs.ForEach(x =>
                {
                    if(model.DocInfo.Any(y => y.Contains(x.DocumentTypeId.ToString())))
                        _applicationTypeDocs.Delete(x);
                });
                
                foreach (var item in model.DocInfo)
                {
                    var doc = item.Split("|");
                    _applicationTypeDocs.InsertDocs(new ApplicationTypeDocuments
                    {
                        ApplicationTypeId = model.ApplicationTypeId,
                        DocumentTypeId = int.Parse(doc[0]),
                        DocType = doc[1]
                    });
                }

            }

            return RedirectToAction("ApplicationTypes");
        }

        public IActionResult StaffDesk()
        {
            var users = new List<ApplicationUser>();
            if (User.IsInRole(Roles.Planning))
                users = _userManager.GetUsersInRoleAsync(Roles.Planning).Result.ToList();
            else
                users = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role)
                            .Where(x => x.IsActive && !x.Email.Equals(User.Identity.Name) 
                            && !x.UserRoles.FirstOrDefault().Role.Name.Equals("Support")
                            && !x.UserRoles.FirstOrDefault().Role.Name.Contains("Admin") && x.IsActive).ToList();
            return View(users);
        }

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

        public IActionResult ApplicationFlow() => View(_workFlow.GetAll());

        private void SendMail(Application application, string comment, string subject, string body, string action, string mailtype, Message message, string ActingStaffEmail = null)
        {
            var tk = $"Application for {mailtype} with reference: {application.Reference} on <br/>NUPRC Gas Export Permit portal (GATEX) has been sent to you for further action: " +
                     $"<br /> {comment}. <br/>";

            if(ActingStaffEmail != null)
            {
                message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, $"https://gatex.nuprc.gov.ng/account/login?email={ActingStaffEmail}");

                _message.Update(message);
                Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(),
                    ActingStaffEmail,
                    message.Subject, message.Content);
            }
            else
            {
                message.Content = string.Format(body, message.Subject, tk, message.Id, DateTime.Now.Year, $"https://gatex.nuprc.gov.ng/account/login?email={application.LastAssignedUserId}");

                _message.Update(message);
                Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(),
                    application.LastAssignedUserId,
                    message.Subject, message.Content);
            }
        }

        public IActionResult EditFlow(int id) => View(_workFlow.GetAll().FirstOrDefault(x => x.Id == id));
        [HttpPost]
        public ActionResult EditFlow(WorkFlow model)
        {
            if (model != null)
                _workFlow.Update(model);
            return RedirectToAction("ApplicationFlow");
        }
        
        public IActionResult AddFlow(int id) => View();
        [HttpPost]
        public ActionResult AddFlow(WorkFlow model)
        {
            if (model != null)
                _workFlow.Insert(model);
            return RedirectToAction("ApplicationFlow");
        }

        public IActionResult DeleteFlow(int id)
        {
            _workFlow.Remove(id);
            return RedirectToAction("ApplicationFlow");
        }

        public IActionResult PaymentEvidences() => View(_paymentEvidence.GetAll());

        public IActionResult EditPaymentEvidence(int id) => View(_paymentEvidence.FindById(id));

        [HttpPost]
        public IActionResult EditPaymentEvidence(PaymentEvidence model) 
        {
            if (ModelState.IsValid)
            {
                var payment = _paymentEvidence.FindById(model.Id);
                if(payment != null)
                {
                    payment.Amount = model.Amount;
                    payment.Description = model.Description;
                    payment.ReferenceCode = model.ReferenceCode;
                    payment.UsageCount = model.UsageCount;
                    payment.ApplicationQuantity = model.ApplicationQuantity;
                    
                    _paymentEvidence.Update(payment);
                }
            }
            return RedirectToAction("PaymentEvidences");
        }

        public IActionResult GasStreams() => View(_product.GetAll());

        public IActionResult EditProduct(int id) => View(_product.FindById(id));

        [HttpPost]
        public IActionResult EditProduct(Product model) 
        {
            var product = _product.FindById(model.Id);
            if(product != null)
            {
                product.Name = model.Name;
                _product.Update(product);
            }
            return RedirectToAction("GasStreams");
        }

        public async Task<IActionResult> StaffApps(string id)
        {
            if (!string.IsNullOrEmpty(id)) 
            { 
                var user = await _userManager.FindByIdAsync(id);
                if(user != null)
                {
                    var apps = _application.GetStappApps(user.Email);
                    ViewData["StaffEmail"] = apps?.FirstOrDefault().LastAssignedUserId;
                    return View(apps);
                }
            }
            return RedirectToAction("StaffDesk");
        }

        [HttpPost]
        public async Task<IActionResult> StaffApps(string email, List<int> ApplicationId)
        {
            if(ApplicationId.Count > 0 && !string.IsNullOrEmpty(email))
            {
                var apps = _application.GetAll().Where(x => ApplicationId.Contains(x.Id) && x.LastAssignedUserId.Equals(email)).ToList();
                if(apps.Any())
                {
                    var staff = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(e => e.Email.Equals(email));
                    var list = await _userManager.GetUsersInRoleAsync(staff.UserRoles.FirstOrDefault().Role.Name);
                    list = list.Where(x => x.IsActive && !x.Email.Equals(email)).ToList();
                    if (list.Count > 0)
                    {
                        if (list.Count == 1)
                            apps.ForEach(x => { x.LastAssignedUserId = list.FirstOrDefault().Email; });
                        else
                        {
                            for(var i = 0; i < apps.Count; i++)
                            {
                                var user = list.OrderBy(l => l.LastJobDate).FirstOrDefault();
                                apps[i].LastAssignedUserId = user.Email;
                                user.LastJobDate = DateTime.UtcNow.AddHours(1);
                                await _userManager.UpdateAsync(user);
                            }
                        }
                        _application.UpdateList(apps);
                        TempData["Message"] = "APplications on staff desk have been distributed accordingly.";
                    }
                }
            }
            return RedirectToAction("StaffDesk");
        }
    }
}