using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System;
using GOTEX.Core.BusinessObjects;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GOTEX.Core.Utilities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Options;

namespace GOTEX.HostedServices
{
    public class ApplicationDelayService : BackgroundService
    {
        private IServiceScopeFactory _serviceScope;
        private IWebHostEnvironment _hostingEnvironment;
        private readonly EmailSettings _emailSettings;

        public ApplicationDelayService(
            IServiceScopeFactory serviceScope,
            IWebHostEnvironment hostingEnvironment,
            IOptions<EmailSettings> options)
        {
            _serviceScope = serviceScope;
            _hostingEnvironment = hostingEnvironment;
            _emailSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScope.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        var appsDelayed = await db.Applications
                            .Include(h => h.Histories).Include(u => u.User)
                            .Where(x => !string.IsNullOrEmpty(x.LastAssignedUserId) 
                                    && !x.User.Email.Equals(x.LastAssignedUserId) 
                                    && x.Histories.OrderByDescending(i => i.Id)
                                    .FirstOrDefault(d => d.ProcessingUser.Equals(x.LastAssignedUserId) 
                                    && d.DateAssigned.AddHours(72) < DateTime.UtcNow.AddHours(1)) != null)
                            .ToListAsync();
                        if (appsDelayed.Any())
                        {
                            var managers = new List<string>();
                            var users = _userManager.Users.Include(ur => ur.UserRoles).ThenInclude(r => r.Role).Where(x => x.CompanyId == null && x.IsActive);
                            for (int i = 0; i < appsDelayed.Count; i++)
                            {
                                var user = users.FirstOrDefault(x => x.Email.Equals(appsDelayed[i].LastAssignedUserId));
                                if (user.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.Inspector))
                                    managers = await users.Where(x => x.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.Supervisor)).Select(e => e.Email).ToListAsync();
                                else if (user.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.Supervisor))
                                    managers = await users.Where(r => r.UserRoles.Any(x => x.Role.Equals(Roles.ADCOGTO) || x.Role.Equals(Roles.ECDP))).Select(e => e.Email).ToListAsync();
                                else if (user.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.ADCOGTO) || user.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.ECDP))
                                    managers = await users.Where(r => r.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.CCE)).Select(e => e.Email).ToListAsync();
                                else if (user.UserRoles.FirstOrDefault().Role.Name.Equals(Roles.Planning))
                                    managers = await users.Where(r => r.UserRoles.Any(x => x.Role.Name.Equals(Roles.Inspector))).Select(e => e.Email).ToListAsync();

                                if (managers.Any())
                                {
                                    var template = Utils.ReadTextFile(_hostingEnvironment.WebRootPath, "GeneralFormat.txt");
                                    var subject = "Application Delay Notification";
                                    var message = $"This is to notify you that an application with reference number - {appsDelayed[i].Reference} have been on ({user.FirstName} {user.LastName}) {user.Email}'s desk for over 72hours.";
                                    string content = string.Format(template, subject, message, "", DateTime.Now.Year, "https://gatex.nuprc.gov.ng");

                                    Utils.SendMail(_emailSettings.Stringify().Parse<Dictionary<string, string>>(), "", subject, content, null, managers);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                await Task.Delay(1000 * 60 * 60 ^ 24, stoppingToken);
            }
        }
    }
}
