using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
//using System.Web.Mvc;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using GOTEX.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace GOTEX.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IElpsRepository _elps;
        private IAppConfiguration<Configuration> _appConfig;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(
            IElpsRepository elps,
            IAppConfiguration<Configuration> appConfig,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _elps = elps;
            _appConfig = appConfig;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET
        // public IActionResult Index()
        // {
        //     return View();
        // }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var scheme = HttpContext.Request.Scheme;
                var ip = HttpContext.Connection.RemoteIpAddress.ToString();
                var hash = Utils.GenerateSHA512($"{_appConfig.GetAppId()}.{model.email}.{_appConfig.GetAppKey()}");

                //if ((scheme.Equals("https") && !ip.Equals("::1") && hash.Equals(model.code))
                //    || (scheme.Equals("https") && ip.Equals("::1") && !hash.Equals(model.code))
                //    || (scheme.Equals("http") && !hash.Equals(model.code)))
                //{
                    var user = await _userManager.FindByNameAsync(model.email);
                    if (user == null)
                    {
                        var company = _elps.GetCompanyDetailByEmail(model.email);
                        if (company?.Count > 0)
                            user = await RegisterCompany(company);
                        // else
                        // {
                        //     user = await CreateAdminUsers(model.email);
                        // }
                    }

                    if (user != null && user.IsActive)
                    {
                        if (User.IsInRole("Staff"))
                        {
                            TempData["ErrorMessage"] = "Access to this portal is denied, please contact ICT/Support.";
                            return RedirectToAction("Index", "Home");
                        }
                        await _signInManager.SignInAsync(user, false);
                        
                        if (!user.ProfileComplete && _userManager.IsInRoleAsync(user, "Company").Result)
                            return RedirectToAction("CompanyProfile", "Dashboard");
                        else if(user.ProfileComplete && _userManager.IsInRoleAsync(user, "Company").Result)
                            return RedirectToAction("Dashboard", "Company");
                        
                        return RedirectToAction("Index", "DashBoard");
                    }
                    else if(user != null && !user.IsActive)
                        TempData["ErrorMessage"] = "Access to this portal is denied, please contact ICT/Support.";
                    else
                        TempData["ErrorMessage"] = "An error occured, please contact Support/ICT.";

                    return await LogOff();
                //}
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("Index", "Home");
        }
        private async Task<ApplicationUser> CreateAdminUsers(string email)
        {
            string[] users = new[] {"planning@dpr.gov.ng|Planning",
                "officerhq@dpr.gov.ng|Inspector", 
                "supervisorhq@dpr.gov.ng|Supervisor", "ad@dpr.gov.ng|CTO",
                "head@dpr.gov.ng|HDS", 
                "directorhq@dpr.gov.ng|OOD",
                "damilare.olanrewaju@brandonetech.com|Admin"
            };
            var user = users.FirstOrDefault(x => x.StartsWith(email));
            var admin = new ApplicationUser();

            if (user != null)
            {
                var item = user.Split('|');
                if (await _userManager.FindByEmailAsync(item[0]) != null) return admin;
                admin = new ApplicationUser
                {
                    Email = item[0],
                    UserName = item[0],
                    EmailConfirmed = true,
                    FirstName = "Test",
                    LastName = "User",
                    ProfileComplete = true,
                    PhoneNumber = "0802547",
                    CompanyId = null,
                    IsActive = true
                };
                await _userManager.CreateAsync(admin);
                await _userManager.AddToRoleAsync(admin, item[1]);
            }
            else
            {
                if (await _userManager.FindByEmailAsync(email) != null) return admin;
                admin = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    ProfileComplete = true,
                    CompanyId = null,
                    IsActive = false
                };
                await _userManager.CreateAsync(admin);
                await _userManager.AddToRoleAsync(admin, "Staff");
            }

            return admin;
        }
        [AllowAnonymous]
        public async  Task<IActionResult> Login(string email)
        {
            var usr = await _userManager.FindByEmailAsync(email);
            if (usr != null)
                await _signInManager.SignInAsync(usr, null, null);

            if (usr.ProfileComplete && _userManager.IsInRoleAsync(usr, "Company").Result)
                return RedirectToAction("Dashboard", "Company");

            return RedirectToAction("Index", "DashBoard");
        }
        private async Task<ApplicationUser> RegisterCompany(Dictionary<string, string> dic)
        {
            var elpsaddid = dic.GetValue("registered_Address_Id");
            var company = new Company
            {
                Name = dic.GetValue("name"),
                Nationality = dic.GetValue("nationality"),
                ElpsId = int.Parse(dic.GetValue("id")),
                RcNumber = dic.GetValue("rC_Number"),
                TinNumber = dic.GetValue("tin_Number"),
                YearIncorporated = dic.GetValue("year_Incorporated"),
                RegAddressId = string.IsNullOrEmpty(elpsaddid) ? 0 : int.Parse(elpsaddid)
            };
            var user = new ApplicationUser
            {
                UserName = dic.GetValue("user_id"),
                Email = dic.GetValue("user_id"),
                EmailConfirmed = true,
                PhoneNumber = dic.GetValue("contact_Phone"),
                ProfileComplete = false,
                FirstName = dic.GetValue("contact_firstname"),
                LastName = dic.GetValue("contact_lastname"),
                Company = company,
                IsActive = true
            };
            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, "Company");
            
            return user;
        }
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _signInManager.SignOutAsync();
            var elpsLogOffUrl = $"{_appConfig.GetElpsUrl()}/Account/RemoteLogOff";
            var returnUrl = $"{Request.Scheme}://{Request.Host}";
            var frm = "<form action='" + elpsLogOffUrl + "' id='frmTest' method='post'>" + "<input type='hidden' name='returnUrl' value='" + returnUrl + "' />" + "<input type='hidden' name='appId' value='" + _appConfig.GetAppKey() + "' />" + "</form>" + "<script>document.getElementById('frmTest').submit();</script>";
            return Content(frm, "text/html");
        }
    }
}