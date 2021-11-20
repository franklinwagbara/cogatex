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
using Microsoft.EntityFrameworkCore;
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
        private readonly IApplication<Application> _application;
        private readonly IAppHistory<ApplicationHistory> _history;

        public AccountController(
            IElpsRepository elps,
            IAppConfiguration<Configuration> appConfig,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IApplication<Application> application,
            IAppHistory<ApplicationHistory> history)
        {
            _elps = elps;
            _appConfig = appConfig;
            _userManager = userManager;
            _signInManager = signInManager;
            _application = application;
            _history = history;
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
                var hash = Utils.GenerateSHA512($"{_appConfig.GetAppId()}.{model.email}.{_appConfig.GetAppKey()}");
                var user = new ApplicationUser();
                // if (hash.Equals(model.code))
                // {
                    //fetch email as company
                    var dic = _elps.GetCompanyDetailByEmail(model.email);
                    if (dic.Count > 0)
                    {
                        user = _userManager.Users.Include(x => x.Company)
                            .FirstOrDefault(x => x.Company.ElpsId == int.Parse(dic.GetValue("id")));
                        if (user != null)
                        {
                            var apps = _application.GetAll().Where(x => x.LastAssignedUserId.Equals(user.Email)).ToList();
                            var history1 = (from h in _history.All() where h.CurrentUser.Equals(user.Email) select h).ToList();
                            var history2 = (from h in _history.All() where h.ProcessingUser.Equals(user.Email) select h).ToList();

                            apps.ForEach(x => x.LastAssignedUserId = model.email);
                            
                            user.Email = model.email.Trim();
                            user.UserName = model.email.Trim();
                            user.Company.Name = dic.GetValue("name");
                            user.Company.Nationality = dic.GetValue("nationality");
                            user.Company.RcNumber = dic.GetValue("rC_Number");
                            user.Company.TinNumber = dic.GetValue("tin_Number");
                            user.Company.YearIncorporated = dic.GetValue("year_Incorporated");

                            //apps2.ForEach(x => x.CompanyUserId = Email.Trim());
                            history1.ForEach(x => x.CurrentUser = model.email);
                            history2.ForEach(x => x.ProcessingUser = model.email);
                            
                            if(apps.Count > 0)
                                _application.UpdateList(apps);
                            if(history1.Count > 0)
                                _history.UpdateList(history1);
                            if(history2.Count > 0)
                                _history.UpdateList(history2);

                            await _userManager.UpdateAsync(user);
                        }
                        else
                            user = await RegisterCompany(dic);
                    }
                    else
                    {
                        //fetch email as staff
                        var staff = _elps.GetStaff(model.email);
                        if (staff != null)
                        {
                            var str = model.email.Split('@');
                            user = _userManager.Users.FirstOrDefault(x => x.Email.StartsWith(str.FirstOrDefault()));
                            if (user != null && !user.Email.Equals(model.email))
                            {
                                //fetch apps
                                var apps = _application.GetAll().Where(x => x.LastAssignedUserId.Equals(user.Email)).ToList();
                                apps.ForEach(x => x.LastAssignedUserId = model.email);
                                //fetch apphistory
                                var apphistory1 = _history.All().Where(x => x.ProcessingUser.Equals(user.Email)).ToList();
                                if(apphistory1.Count > 0)
                                    apphistory1.ForEach(x => x.ProcessingUser = model.email);
                                
                                var apphistory2 = _history.All().Where(x => x.CurrentUser.Equals(user.Email)).ToList();
                                if(apphistory2.Count > 0)
                                    apphistory2.ForEach(x => x.CurrentUser = model.email);

                                
                                user.Email = model.email;
                                user.UserName = model.email;

                                await _userManager.UpdateAsync(user);
                                
                                if(apps.Count > 0)
                                    _application.UpdateList(apps);
                                if(apphistory1.Count > 0)
                                    _history.UpdateList(apphistory1);
                                if(apphistory2.Count > 0)
                                    _history.UpdateList(apphistory2);
                            }
                        }
                    }
                    if (user is { IsActive: true })
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

        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                var response = _elps.ChangePassword(new
                {
                    model.OldPassword,
                    model.NewPassword,
                    ConfirmPassword = model.CPassword
                }, User.Identity.Name);

                if (response != null)
                {
                    if (response.GetValue("msg").Equals("ok", StringComparison.OrdinalIgnoreCase) &&
                        response.GetValue("code").Equals("1"))
                    {
                        TempData["Message"] = "Password changed successfully, please login again to continue";
                        return await LogOff();
                    }
                    TempData["Message"] = "Password change unsuccessful, please try again";

                }
            }
            return RedirectToAction("Index", "DashBoard");
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