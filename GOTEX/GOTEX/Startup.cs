using System;
using System.Threading.Tasks;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.DAL;
using GOTEX.Core.Utilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;

namespace GOTEX
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, 
                m => m.MigrationsAssembly("GOTEX")));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AppDbContext>();

            
            services.ServiceInjections();
            
            services.Configure<EmailSettings>(Configuration.GetSection("AppSettings").GetSection("EmailSettings"));
            services.Configure<ConstantDocument>(Configuration.GetSection("AppSettings").GetSection("ConstantDocument"));
            services.Configure<RemitaPartners>(Configuration.GetSection("AppSettings").GetSection("RemitaPartners"));
            
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);//You can set Time   
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(config =>
            {
                config.Cookie.HttpOnly = true;
                config.Cookie.Name = "Cookies";  
                config.LoginPath = "/Account/Login";  
                config.CookieManager = new ChunkingCookieManager();
            });
            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(1);
            });
            // services.AddAuthentication();
            
            services.AddServerSideBlazor();    
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.AspNetCore.Hosting.IHostingEnvironment env2, IServiceProvider serviceProvider, RoleManager<ApplicationRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
            }); 
            CreateRoles(serviceProvider, roleManager).Wait();
            RotativaConfiguration.Setup(env2);
        }

        private async Task CreateRoles(IServiceProvider serviceProvider, RoleManager<ApplicationRole> roleManager)
        {
            roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var roles = new[]
            {
                Roles.Company,
                Roles.Planning,
                Roles.Inspector,
                Roles.Supervisor,
                Roles.CTO,
                Roles.HDS,
                Roles.OOD,
                Roles.Support,
                Roles.Admin,
                Roles.Staff,
                Roles.ICT,
                Roles.HGMR,
                Roles.ACE
            };
            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new ApplicationRole { Name = role});
            }
        }
    }
}