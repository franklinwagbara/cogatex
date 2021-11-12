using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.DAL;
using GOTEX.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GOTEX.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GOTEX.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInMnaager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public HomeController(ILogger<HomeController> logger,
            IAppConfiguration<Configuration> appConfiguration,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser>  userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _signInMnaager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ViewData["Message"] = TempData["Message"];
                return View();
            }
            else
                return RedirectToAction("Login", "Account", new{ email = User.Identity.Name });
        } 
        public IActionResult Privacy() => View();
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}