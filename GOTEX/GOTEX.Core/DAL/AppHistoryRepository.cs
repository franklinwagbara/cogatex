using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using GOTEX.Core.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GOTEX.Core.DAL
{
    public class AppHistoryRepository : IAppHistory<ApplicationHistory>
    {
        private AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        
        public AppHistoryRepository(
            AppDbContext context,  
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public bool CreateNextProcessingPhase(Application application, string action, string comment = null)
        {
            try
            {
                var processingUser = _userManager.Users
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role).FirstOrDefault(x => x.Email.Equals(application.LastAssignedUserId));
                var nextaction = _context.WorkFlows.FirstOrDefault(x =>
                    x.Action.ToLower().Contains(action) && x.TriggeredByRole.ToLower().Equals(processingUser.UserRoles.FirstOrDefault().Role.Name.ToLower()));
                var nextprocofficer = nextaction.TargetRole.Equals("Company") ? application.User : GetNextProcessingOfficer(nextaction.TargetRole, action, application, processingUser);
                if (nextprocofficer != null)
                {
                    var history = new ApplicationHistory
                    {
                        Action = action,
                        Comment = string.IsNullOrEmpty(comment)
                            ? $"{AppHistoryComment(nextaction)}"
                            : $"{AppHistoryComment(nextaction)} => {comment}",
                        Status = !action.ToLower().Contains("approve")
                            ? nextaction.Status
                            : nextaction.Status + GetApplicationStatus(action, processingUser.UserRoles.FirstOrDefault().Role.Name),
                        CurrentUser = processingUser.UserName,
                        CurrentUserRole = processingUser.UserRoles.FirstOrDefault().Role.Name,
                        DateAssigned = DateTime.UtcNow.AddHours(1),
                        DateTreated = DateTime.UtcNow.AddHours(1),
                        IsAssigned = true,
                        Application = application,
                        ApplicationId = application.Id,
                        ProcessingUserRole = nextaction.TargetRole,
                        ProcessingUser = nextprocofficer.Email,
                        AutoPushed = action.ToLower().Contains("reject") ? false : true,
                        WorkFlowId = nextaction.Id,
                    };
                    _context.ApplicationHistories.Add(history);
                    application.LastAssignedUserId = nextprocofficer.Email;
                    application.StageId = nextaction.Id;
                    if (action.Equals("submitapplication", StringComparison.OrdinalIgnoreCase)
                        || action.Equals("resubmitpayment", StringComparison.OrdinalIgnoreCase)
                        || action.Equals("rejectpayment", StringComparison.OrdinalIgnoreCase)
                        || action.Equals("confirmpayment", StringComparison.OrdinalIgnoreCase)
                        || action.Equals("acceptpayment", StringComparison.OrdinalIgnoreCase)
                        || action.Equals("submitpayment", StringComparison.OrdinalIgnoreCase)
                        || action.Equals("resubmitapplication", StringComparison.OrdinalIgnoreCase))
                        application.Status = nextaction.Status;
                    else
                        application.Status = GetApplicationStatus(action, processingUser.UserRoles.FirstOrDefault().Role.Name);
                    _context.Applications.Update(application);
                    _context.SaveChanges();
                    return true;
                }

            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Action = action,
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = application.LastAssignedUserId
                });
            }
            return false;
        }
        private string AppHistoryComment(WorkFlow flow)
        {
            var comment = string.Empty;
            switch (flow.Action.ToLower())
            {
                case "submitapplication":
                    comment = "Application initiated by company";
                    break;
                case "submitpayment":
                    comment = "Application submitted successfully, awaiting Planning approval";
                    break;
                case "resubmitapplication":
                    comment = "Application re-submitted by company";
                    break;
                case "confirmpayment":
                    comment = "Application payment confirmed by Planning team";
                    break;
                case "rejectpayment":
                    comment = "Application payment was rejected by Planning team";
                    break;
                case "resubmitpayment":
                    comment = "Application payment re-submitted successfully, awaiting Planning approval";
                    break;
                case "acceptpayment":
                    comment = "Application payment re-submission confirmed by Planning team";
                    break;
                case "approve":
                    comment = $"Application accepted by {flow.TriggeredByRole}";
                    break;
                case "reject":
                    comment = $"Application rejected by {flow.TriggeredByRole}";
                    break;
                default:
                    break;
            }
            return comment;
        }
        private string GetApplicationStatus(string action, string role)
        {
            if (action.ToLower().Contains("approve"))
            {
                if (role.Equals("Inspector") || role.Equals("Supervisor") || role.Equals("ADCOGTO") 
                    || role.Equals("HGMR")|| role.Equals("HDS") || role.Equals("Reviewer") || role.Equals("ECDP"))
                    return ApplicationStatus.Processing;
                else if(role.Equals("OOD")|| role.Equals("ACE") || role.Equals("Director") || role.Equals("CCE") || role.Equals("CCE_STA"))
                    return ApplicationStatus.Completed;
            }
            return ApplicationStatus.Rejected;
        }
        public ApplicationUser GetNextProcessingOfficer(string rolename, string action, Application application, ApplicationUser currentuser)
        {
            var nextofficer = new ApplicationUser();
            try
            {
                if (action.ToLower().Contains("reject"))
                {
                    var history = _context.ApplicationHistories.Where(x =>
                        x.CurrentUserRole.Equals(rolename) && x.ApplicationId == application.Id && x.ProcessingUser == currentuser.Email)
                        .OrderByDescending(y => y.Id).FirstOrDefault();
                    
                    if (history == null && _userManager.IsInRoleAsync(currentuser, "Inspector").Result)
                        nextofficer = _userManager.FindByIdAsync(application.UserId).Result;
                    else
                        nextofficer = _userManager.FindByNameAsync(history.CurrentUser).Result;
                }
                else
                {
                    var users = _userManager.GetUsersInRoleAsync(rolename).Result;
                    var deskcount = new Dictionary<string, int>();
                    if (users.Where(x => x.IsActive).ToList().Count > 1)
                    {
                        foreach (var item in users)
                        {
                            if (item.IsActive)
                            {
                                int count = _context.Applications.Count(x => x.Status != ApplicationStatus.Completed 
                                                                             && x.LastAssignedUserId == item.Email);
                            
                                deskcount.Add(item.Email, count);
                            }
                        }

                        var officerEmail = deskcount.OrderBy(x => x.Value).FirstOrDefault().Key;
                        nextofficer = users.FirstOrDefault(x => x.Email == officerEmail);
                    }
                    else
                        nextofficer = users.FirstOrDefault(x => x.IsActive);
                    
                }
            }
            catch (Exception ex)
            {
                _context.Logs.Add(new Log
                {
                    Action = "Get Next Processing Officer",
                    Date = DateTime.UtcNow.AddHours(1),
                    Error = $"{ex.Message}\n{ex.InnerException}\n{ex.StackTrace}",
                    UserId = application.LastAssignedUserId
                });
            }
            return nextofficer;
        }
        public List<ApplicationHistory> GetApplicationHistoriesById(int applicationid) => 
            _context.ApplicationHistories.Where(x => x.ApplicationId == applicationid).ToList();

        public void Update(ApplicationHistory item)
        {
            _context.ApplicationHistories.Update(item);
            _context.SaveChanges();
        }

        public List<ApplicationHistory> SentApplications(string email)
            =>  All().Where(x => x.CurrentUser.Equals(email))
                .OrderByDescending(x => x.DateAssigned).ToList();

        public List<ApplicationHistory> All() => _context.ApplicationHistories
            .Include("Application.User.Company")
            .Include("Application.Product")
            .Include("Application.ApplicationType")
            .Include("Application.Terminal")
            .Include("Application.Quarter").ToList();

        public bool UpdateList(List<ApplicationHistory> itemlist)
        {
            _context.ApplicationHistories.UpdateRange(itemlist);
            _context.SaveChanges();
            return true;
        }
    }
}