using System;
using System.Linq;
using System.Threading.Tasks;
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
    public class StaffController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IApplication<Application> _application;
        private IAppHistory<ApplicationHistory> _history;
        private IElpsRepository _elps;
        private IRepository<Message> _message;
        
        public StaffController(
            UserManager<ApplicationUser> userManager,
            IApplication<Application> applicataion,
            IAppHistory<ApplicationHistory>  history,
            IElpsRepository elps,
            IRepository<Message> message)
        {
            _userManager = userManager;
            _application = applicataion;
            _history = history;
            _elps = elps;
            _message = message;
        }
        // GET
        public IActionResult Index() => View();
        public IActionResult All()
        {
            ViewData["Message"] = TempData["Message"];
            ViewData["alert"] = TempData["alert"];
            return View( _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Where(x => !x.UserRoles.FirstOrDefault().Role.Name.Equals("Company")).ToList());
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var apps = _application.GetAll().Where(x => x.LastAssignedUserId.Equals(user.Email)).ToList();
            if (apps.Count > 0 && apps.Any(x => x.Status != ApplicationStatus.Completed) && !model.IsActive)
            {
                TempData["Message"] = $"User cannot be disabled, {apps.Count} application(s) currently on staff's desk";
                TempData["alert"] = "alert-danger";
                return RedirectToAction("All");
            }
            if (user != null)
            {
                user.IsActive = model.IsActive;
                await _userManager.UpdateAsync(user);
            }
            
            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                if(role != null)
                    await _userManager.RemoveFromRoleAsync(user, role);
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            TempData["alert"] = "alert-success";
            TempData["Message"] = "User updated successfully";
            return RedirectToAction("All");
        }
        public IActionResult GetRoles()
        {
            var roles = new[]
            {
                Roles.Company,
                Roles.Planning,
                Roles.Inspector,
                Roles.Supervisor,
                Roles.CTO,
                Roles.HDS,
                Roles.HGMR,
                Roles.ACE,
                Roles.OOD,
                Roles.Support,
                Roles.Admin,
                Roles.Staff,
                Roles.ICT,
                Roles.ACE_STA,
                Roles.ED_STA,
                Roles.ECDP,
                Roles.CCE,
                Roles.CCE_STA,
                Roles.ADCOGTO
            };
            return Json(new { roles });
        }
        public async Task<IActionResult> GetStaffRoleList(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            return Json(new{ users = _userManager.GetUsersInRoleAsync(role).Result
                .Where(x => x.IsActive && !x.Email.Equals(email.Trim())).ToList()});
        }
        public IActionResult Relief(StaffReliefViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Message"] = "One or more fields is/are invalid, please try again";
                    TempData["alert"] = "alert-warning";
                }
                else
                {
                    var applications = _application.GetAll().Where(x
                        => x.LastAssignedUserId.Equals(model.OldStaffEmail)).Take(model.AppNumber).ToList();

                    if (applications.Count > 0)
                    {
                        foreach (var application in applications)
                        {
                            var lasthistory = _history.GetApplicationHistoriesById(application.Id)
                                .OrderByDescending(x => x.Id).FirstOrDefault();
                            
                            application.LastAssignedUserId = model.NewStaff;
                            _application.Update(application);

                            if (lasthistory != null)
                            {
                                lasthistory.ProcessingUser = model.NewStaff;
                                _history.Update(lasthistory);
                            }
                        }
                    };

                }
            }
            catch (Exception ex)
            {
                
            }
            return RedirectToAction("All");
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = model.Stringify().Parse<ApplicationUser>();
                    user.ProfileComplete = true;
                    user.UserName = model.Email;
                    user.EmailConfirmed = true;
                    user.PhoneNumberConfirmed = true;

                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
            }
            catch (Exception ex)
            {
                
            }
            TempData["alert"] = "alert-success";
            TempData["Message"] = "Staff created successfully";
            return RedirectToAction("All");
        }

        public async Task<IActionResult> Delete(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var messages = _message.GetAll().Where(x => x.UserId.Equals(email)).ToList();
                if (messages != null)
                    _message.DeleteRange(messages);
                await _userManager.DeleteAsync(user);
            }
            
            TempData["alert"] = "alert-success";
            TempData["Message"] = "Staff deleted successfully";
            return RedirectToAction("All");
        }
        public IActionResult GetStaffDetail(string email) => Json(new {staff = _elps.GetStaff(email)});
    }
}