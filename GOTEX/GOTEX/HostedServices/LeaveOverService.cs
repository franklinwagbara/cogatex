using GOTEX.Core.BusinessObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GOTEX.HostedServices
{
    public class LeaveOverService : BackgroundService
    {
        private readonly IHostEnvironment _environment;
        private IServiceScopeFactory _scopeFactory;
        private int Delay => 1 * 60 * 60 * 1000;

        public LeaveOverService(IHostEnvironment environment, IServiceScopeFactory scopeFactory)
        {
            _environment = environment;
            _scopeFactory = scopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var leaves = dbContext.Leaves.Where(x => !x.IsDeleted).ToList();

                        leaves = leaves.Where(x => x.End <= DateTime.Now).ToList();

                        leaves.ForEach(leave =>
                        {
                            leave.IsDeleted = true;
                            dbContext.Leaves.Update(leave);
                        });
                    }
                }
                catch (System.Exception)
                {
                }
                await Task.Delay(Delay, stoppingToken);
            }
        }
    }
}
