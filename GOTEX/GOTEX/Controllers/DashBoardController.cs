using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
///using System.Web.Mvc;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using GOTEX.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace GOTEX.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IAppConfiguration<Configuration> _appConfig;
        private IElpsRepository _elps;
        private readonly AppDbContext _context;
        private IRepository<Message> _message;
        private IApplication<Application> _appRepository;
        private IApplication<Application> _application; 
        private readonly IAppHistory<ApplicationHistory> _history;

        public DashBoardController(
            UserManager<ApplicationUser> userManager, 
            IElpsRepository elps, 
            IAppConfiguration<Configuration> appConfig,
            AppDbContext context,
            IRepository<Message> message,
            IApplication<Application> appRepository,
            IApplication<Application> application,
            IAppHistory<ApplicationHistory> history)
        {
            _userManager = userManager;
            _elps = elps;
            _appConfig = appConfig;
            _context = context;
            _message = message;
            _appRepository = appRepository;
            _application = application;
            _history = history;
        }
        public async Task<IActionResult> Index(DashboardViewModel model)
        {
            ViewData["Message"] = TempData["Message"];
            int declined = 0;
            int allApps = 0;
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var allapps = _appRepository.GetAll();
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

            if (User.IsInRole("Company"))
                return RedirectToAction("Dashboard", "Company");
            else
            {
                if (User.IsInRole("Planning"))
                    return RedirectToAction("Index", "Admin");
                else if(User.IsInRole("ACE_STA") || User.IsInRole("ED_STA") || User.IsInRole(Roles.OOCCE) || User.IsInRole(Roles.CCE_STA))
                    return RedirectToAction("AceDesk", "DashBoard");

                model.All = allapps.Count;
                model.Processing = allapps.Count(x => x.Status.ToLower().Equals("processing") || x.Status.ToLower().Equals("payment confirmed"));
                model.Declined = apps.Count(x => x.Action.ToLower().Equals("reject"));
                model.MyDesk = allapps.Count(x =>
                    x.LastAssignedUserId == user.Email && x.Status != ApplicationStatus.Completed);
                model.Approved = allapps.Count(x => x.Status.ToLower().Equals("completed"));
                model.AppsTreated = apps.Count(a => a.Action.ToLower().Equals("approve"));
            }
            model.Messages = _message.GetListByUserId(user.Id).OrderByDescending(x => x.Date).Take(5).ToList();
            return View(model);
        }
        
        public IActionResult Company()
        {
            throw new System.NotImplementedException();
        }
        
        public IActionResult CompanyProfile(string userid = null)
        {
            var user = _userManager.Users.Include("Company").FirstOrDefault( x => x.UserName == User.Identity.Name);
            if (!string.IsNullOrEmpty(userid))
                user = _userManager.Users.Include("Company").FirstOrDefault(x => x.UserName == userid);
        
            var countries = _appConfig.GetCountries().Stringify().Parse<List<Nationality>>();
            ViewBag.ProfileComplete = user.ProfileComplete;
            var company = _elps.GetCompanyDetailByEmail(user.Email).Stringify().Parse<CompanyModel>();
            var companyAdd = new RegisteredAddress();
            if (user.Company.RegAddressId > 0)
                companyAdd = _elps.GetCompanyRegAddressById(user.Company.RegAddressId).Stringify()
                    .Parse<RegisteredAddress>();
            else
                companyAdd = _elps.GetCompanyRegAddress(user.Company.ElpsId).FirstOrDefault();
            //var companyDir = _elps.GetCompanyDirectors(user.Company.ElpsId);

            ViewBag.Email = user.Email;
            ViewBag.Nationality = countries.GetSortedCountry(user.Company.Nationality);
            //ViewBag.Directors = companyDir;
            ViewBag.DirSignature = company.CEOSignature ?? user.Company.DirectorSignature;

            return View(new CompanyInformation
            {
                Company = company,
                RegisteredAddress = companyAdd
            });
        }
        [HttpPost]
        public async Task<IActionResult> CompanyProfile(CompanyInformation model, string oldemail, string Email)
        {
            var user = _context.Users.Include("Company").FirstOrDefault(x => x.Email == User.Identity.Name);
            if (await _userManager.IsInRoleAsync(user, "Support") 
                || await _userManager.IsInRoleAsync(user, "ICT")
                || await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var comapnyuser = _context.Users.Include("Company").FirstOrDefault(x => x.Email == oldemail);
                model.Company.id = comapnyuser.Company.ElpsId;
               _elps.UpdateCompanyDetails(model.Company, Email, true)?.Stringify()?.Parse<CompanyModel>();
                
                if(!Email.Equals(comapnyuser.Email, StringComparison.OrdinalIgnoreCase));
                {
                    comapnyuser.Email= Email;
                    comapnyuser.NormalizedEmail = Email;
                    comapnyuser.NormalizedUserName = Email;
                    comapnyuser.UserName = Email;
                }
                comapnyuser.Company.Name = model.Company.name;
                comapnyuser.Company.RcNumber = model.Company.rC_Number;
                comapnyuser.Company.TinNumber = model.Company.tin_Number;
                    
                _context.Users.Update(comapnyuser);
                _context.SaveChanges();
                
                var applications = _application.GetListByUserId(user.Id);

                foreach (var application in applications)
                {
                    if (application.LastAssignedUserId.Equals(oldemail))
                    {
                        application.LastAssignedUserId = Email;
                        _application.Update(application);
                    }
                }  

                TempData["Message"] = "Company was updated successfully";

                return RedirectToAction("CompanyList", "Admin");
            }
            else if (await _userManager.IsInRoleAsync(user, "Company"))
            {
                if (model.Company != null)
                {
                    user = MapUserCompanyModel(user, model);
                    model.Company.id = user.Company.ElpsId;
                    user.Company.DirectorSignature = model.Company.CEOSignature;
                    model.Company = _elps.UpdateCompanyDetails(model.Company, user.Email, false)?.Stringify()?.Parse<CompanyModel>();
                }
                else if (model.RegisteredAddress != null)
                {
                    model.RegisteredAddress.country_Id = 156;
                    model.RegisteredAddress.type = "Registered";
                    model.RegisteredAddress.stateId = 2412;
                    var addList = new List<RegisteredAddress>();
                    addList.Add(model.RegisteredAddress);
                    user.Company.RegisteredAddress = model.RegisteredAddress.address_1;
                    if (user.Company.RegAddressId > 0)
                    {
                        model.RegisteredAddress.id = user.Company.RegAddressId;
                        addList.Add(model.RegisteredAddress);
                        var resp = _elps.UpdateCompanyRegAddress(addList);
                    }
                    else
                    {
                        var req = _elps.AddCompanyRegAddress(addList, user.Company.ElpsId).Stringify().Parse<List<RegisteredAddress>>().FirstOrDefault();
                        user.Company.RegisteredAddress = req.address_1;
                        user.Company.RegAddressId = req.id;
                    }
                }
                // else if (model.Director != null)
                // {
                //
                // }
                ViewBag.Message = "Update was saved Successfully";
                ViewBag.DirSignature = user.Company.DirectorSignature;

                if (!string.IsNullOrEmpty(user.Company.DirectorSignature) && user.Company.RegAddressId > 0)
                    user.ProfileComplete = true;
                await _userManager.UpdateAsync(user);
                
                if(user.ProfileComplete)
                    return RedirectToAction("Dashboard", "Company");
            
                return RedirectToAction("CompanyProfile", new{userid = user.Email});
            }

            return RedirectToAction("CompanyProfile");
        }

        private ApplicationUser MapUserCompanyModel(ApplicationUser user, CompanyInformation model)
        {
            user.Company.Name = model.Company.name;
            user.UserName = model.Company.user_Id;
            user.Company.RcNumber = model.Company.rC_Number;
            user.Company.TinNumber = model.Company.tin_Number;
            user.Company.Nationality = model.Company.nationality;
            user.Email = model.Company.user_Id;
            user.FirstName = model.Company.contact_FirstName;
            user.LastName = model.Company.contact_LastName;
            user.PhoneNumber = model.Company.contact_Phone;
            //user.Company.DirectorSignature = model.Company.CEOSignature;
            
            return user;
        }
        
        public async Task SaveCeoSignature(string email, string source)
        {
            var user = _userManager.Users.Include("Company").FirstOrDefault(x => x.Email == email);
            if (user != null)
            {
                user.Company.DirectorSignature = source;
                await _userManager.UpdateAsync(user);
            }
        }

        public IActionResult AceDesk()
        {
            ViewData["Title"] = "ECDP's Desk";
            var user = _userManager.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefault(x => x.UserRoles.FirstOrDefault().Role.Name.Equals("ECDP"));         
            
            if (User.IsInRole("CCE_STA") || User.IsInRole("OOCCE"))
            {
                ViewData["Title"] = "CCE's Desk";
                 user = _userManager.Users
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .FirstOrDefault(x => x.UserRoles.FirstOrDefault().Role.Name.Equals("CCE"));                
            }

            var apps = _application.GetAll().Where(x => !string.IsNullOrEmpty(x.LastAssignedUserId) && x.LastAssignedUserId.Equals(user.Email)).ToList();
            return View(apps);
        }
    }
}