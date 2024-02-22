using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using GOTEX.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GOTEX.Controllers
{
    public class LeaveController : Controller
    {
        private readonly IRepository<Leave> _leaveRepo;
        private readonly IRepository<LeaveRequest> _leaveRequestRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveController(IRepository<Leave> leaveRepo, UserManager<ApplicationUser> userManager, IRepository<LeaveRequest> leaveRequestRepo)
        {
            _leaveRepo = leaveRepo;
            _userManager = userManager;
            _leaveRequestRepo = leaveRequestRepo;   
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var actingStaffs = await GetActingStaffs(user);


            var viewModel = new LeaveFormViewModel
            {
                Leave = new LeaveForm()
                {
                    StaffId = user.Id,
                    StaffName = $"{user.FirstName}, {user.LastName}",
                },
                Users = actingStaffs
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult LeaveRequest()
        {

            var leaveRequests = _leaveRequestRepo.GetAll();
            var user = _userManager.FindByEmailAsync(User.Identity.Name).Result;
            
            leaveRequests = leaveRequests.Where(x => x.ApprovingStaffId == user.Id).ToList();
            
            var viewModel = new LeaveRequestsViewModel
            {
                LeaveRequests = leaveRequests,
            };

            return View(viewModel);
        }

        

        [HttpPost]
        public async Task<IActionResult> Create(LeaveFormViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                model.Users = await GetActingStaffs(user);

                if (!ModelState.IsValid)
                {
                    return View("Index", model);
                }

                var leave = new Leave()
                {
                    StaffId = user.Id,
                    ActingStaffId = model.Leave.ActingStaffId,
                    Start = model.Leave.Start,
                    End = model.Leave.End
                };

                _leaveRepo.Insert(leave);

                if (User.IsInRole(Roles.Planning) || User.IsInRole(Roles.Inspector))
                {
                    var leaveRequest = new LeaveRequest()
                    {
                        LeaveId = leave.Id,
                        ApprovingStaffId = GetNextProcessingLeaveSupervisor(),
                        DateCreated = DateTime.Now,
                    };

                    _leaveRequestRepo.Insert(leaveRequest);
                }
                else
                {
                    leave.IsApproved = true;
                    _leaveRepo.Insert(leave);
                    ViewBag.Message = "Leave Application was created successfully.";
                    return View();
                }

                ViewBag.Message = "Leave application creation failed.";
                return View("Index", model);
            }
            catch (System.Exception e)
            {
                ViewBag.Error = e.Message;
                return View("Index", model);
            }
            
        }

        private string GetNextProcessingLeaveSupervisor()
        {
            try
            {
                var supervisors = _userManager.GetUsersInRoleAsync(Roles.Supervisor).Result;
                Random rnd = new Random();
                return supervisors.OrderBy(x => rnd.Next()).FirstOrDefault().Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<List<ApplicationUser>> GetActingStaffs(ApplicationUser user)
        {
            if(User.IsInRole(Roles.Inspector))
            {
                return _userManager.Users.Where(x => x.UserRoles.Any(x => x.Role.Name.Equals(Roles.Inspector)) && x.Id != user.Id).ToList();
            }
            else if(User.IsInRole(Roles.Planning))
            {
                //return _userManager.GetUsersInRoleAsync(Roles.Planning).Result;
                return _userManager.Users.Where(x => x.UserRoles.Any(x => x.Role.Name.Equals(Roles.Planning)) && x.Id != user.Id).ToList();
            }
            else if (User.IsInRole(Roles.Supervisor))
            {
                return _userManager.Users.Where(x => x.UserRoles.Any(x => x.Role.Name.Equals(Roles.Inspector))).ToList();
            }

            return new List<ApplicationUser>();
        }
    }
}
