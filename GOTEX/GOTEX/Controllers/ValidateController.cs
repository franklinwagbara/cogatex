using System;
using GOTEX.Core.BusinessObjects;
using GOTEX.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GOTEX.Controllers
{
    public class ValidateController : Controller
    {
        private IPermit<Permit> _permit;
        public ValidateController(IPermit<Permit> permit)
        {
            _permit = permit;
        }
        // GET
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string permitno)
        {
            var permit = _permit.FindByPermitNumber(permitno);
            if (permit != null)
            {
                    if(permit.ExpiryDate < DateTime.UtcNow.AddHours(1))
                    {
                        ViewBag.Msg = "The Requested Permit has Expired!";
                        ViewBag.MsgType = "warn";
                    }
                    else
                    {
                        ViewBag.Msg = "The Requested Permit is Valid";
                        ViewBag.MsgType = "pass";
                    }
            }
            else
            {
                ViewBag.Msg = "Permit with the specified Permit Number not found. Enter another number and try again";
            }
            return View(permit);
        }
    }
}