using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using GOTEX.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace GOTEX.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        // GET
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplication<Application> _application;
        private readonly IElpsRepository _elps;
        private readonly AppDbContext _context;
        private readonly IAppConfiguration<Configuration> _appConfig;
        private readonly IPermit<Permit> _permit;
        private readonly IRepository<Message> _message;
        private readonly IRepository<Survey> _survey;
        private readonly IRepository<DeclarationForm> _dform;
        private readonly EmailSettings _emailSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;


        public CompanyController(
            UserManager<ApplicationUser> userManager, 
            IApplication<Application> application,
            IElpsRepository elps,
            AppDbContext context,
            IAppConfiguration<Configuration> appConfig,
            IRepository<Message> message,
            IPermit<Permit> permit,
            IRepository<Survey> survey,
            IRepository<DeclarationForm> dform,
            IOptions<EmailSettings> emailSettings,
            IWebHostEnvironment hostingEnvironment,
            IMapper mapper)
        {
            _userManager = userManager;
            _application = application;
            _elps = elps;
            _context = context;
            _appConfig = appConfig;
            _permit = permit;
            _message = message;
            _survey = survey;
            _dform = dform;
            _emailSettings = emailSettings.Value;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var applications = _application.GetListByUserId(user.Id);
            ViewData["Message"] = TempData["Message"];
            return View(applications);
        }

        public async Task<IActionResult> Dashboard(DashboardViewModel model)
        {
            try
            {
                int declined = 0;
                int allApps = 0;
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var allapps = _application.GetAll();
                ViewData["Message"] = TempData["Message"];
                if (User.IsInRole("Company"))
                {
                    var myapplications = allapps.Where(x => x.UserId == user.Id) .ToList();
                    model.All = myapplications.Count;
                    model.MyDesk = allapps.Count(x =>
                        x.LastAssignedUserId == user.Email && x.Status != ApplicationStatus.Completed);
                    model.Processing = myapplications.Count(x => x.Status.ToLower().Equals("processing"));
                    model.Declined = myapplications.Count(x => x.Status.ToLower().Equals("rejected"));
                    model.Approved = myapplications.Count(x => x.Status.ToLower().Equals("completed"));
                    var permits = _permit.GetCompanyPermits(user.Id).Where(x => x.DateIssued > new DateTime(2023, 07, 01, 00, 00, 00)).Select(x => x.Id);
                    var surveys = _survey.GetAll();
                    int sCnt = 0;
                    foreach (var p in permits)
                    {
                        if(!surveys.Any(s => s.PermitId.Equals(p)))
                            sCnt++;
                    }
                    ViewData["Survey"] = sCnt;
                }
                model.Messages = _message.GetListByUserId(user.Id).OrderByDescending(x => x.Date).Take(5).ToList();
            }
            catch (Exception ex)
            {
                
            }
            return View(model);
        }

        public IActionResult DocumentLibrary(string email, List<CompanyDocument> docs)
        {
            try
            {
                var user =  _userManager.Users.Include("Company").FirstOrDefault(x => x.UserName == User.Identity.Name);
                if(!string.IsNullOrEmpty(email))
                    user = _userManager.Users.Include("Company").FirstOrDefault(x => x.Email == email);
                
                ViewBag.CompanyName = $"{user.Company.Name}";
                docs = _elps.GetCompanyDocuments(user.Company.ElpsId).Stringify().Parse<List<CompanyDocument>>();
                if (docs.Count > 0)
                {
                    foreach (var doc in docs)
                    {
                         doc.Source = doc.Source.StartsWith("http") ? doc.Source : $@"{_appConfig.GetElpsUrl()}/api/{doc.Source}";
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return View(docs);
        }

        public IActionResult MyDesk() => View(_application.GetAll().Where(x => x.LastAssignedUserId == User.Identity.Name && !x.Status.Equals(ApplicationStatus.Completed)).ToList());
        
        public IActionResult GetApplications()
        {
            IEnumerable<Application> allApplications = _application.GetAll();
            
            allApplications = allApplications.Where(a => a.LastAssignedUserId == User.Identity.Name);//&& a.Submitted
            
            try  
            {  
               var draw = HttpContext.Request.Form["draw"].FirstOrDefault();  
  
               // Skip number of Rows count  
               var start = Request.Form["start"].FirstOrDefault();  
  
               // Paging Length 10,20  
               var length = Request.Form["length"].FirstOrDefault();  
  
               // Sort Column Name  
               var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();  
  
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
               return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }  
           catch (Exception ex)  
           {  
               
           } 
            return Json(new { draw = "", recordsFiltered = 0, recordsTotal = 0, data = "" });
        }
        
        public async Task<IActionResult> Permits(string email = null)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (!string.IsNullOrEmpty(email))
                user = await _userManager.FindByEmailAsync(email);
            
            return View(_permit.GetCompanyPermits(user.Id));
        }

        public IActionResult ExportType() => View();

        public IActionResult AddSurvey(int id) 
        { 
            var permit = _permit.FindById(id);
            if(permit != null)
                return View(new Survey { PermitId = id });

            return RedirectToAction("Permits");
        }

        [HttpPost]
        public IActionResult PostSurvey(SurveyViewModel model) 
        {
            if(ModelState.IsValid)
            {
                var survwy = _survey.Insert(_mapper.Map<Survey>(model));
                var emailSettings = _emailSettings.Stringify().Parse<Dictionary<string, string>>();
                emailSettings.Add("Portalbase", $"{Request.Scheme}://{Request.Host}/Validate/ViewSurvey/{survwy.Id}");
                var app = _permit.FindById(model.PermitId);
                if(app != null)
                {
                    _survey.SendApplicationSubmittedMail(app?.Application, emailSettings, _hostingEnvironment.WebRootPath);
                    TempData["Message"] = "Survey form has been submitted successfully.";
                }
            }
            return RedirectToAction("Permits");
            //return Json(new { status = false, message = "An error occured, pls try again or contact support." });
        }

        public IActionResult AllDeclarationnForms() 
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(User.Identity.Name));
            var forms = _dform.GetListByUserId(user.Id);
            return View(forms);
        }
    }
}