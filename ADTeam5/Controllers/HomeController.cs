using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ADTeam5.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Models;
using ADTeam5.ViewModels;

namespace ADTeam5.Controllers
{
    
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;       
        readonly GeneralLogic userCheck;

        public HomeController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            ViewData["UserRole"] = identity[1];

            switch (ViewData["UserRole"].ToString())
            {
                case "Manager":
                    return RedirectToAction("Index", "AdjustmentRecords");
                    break;
                case "Supervisor":
                    return RedirectToAction("Index", "AdjustmentRecords");
                    break;
                case "Clerk":
                    return RedirectToAction("Index", "PurchaseOrderRecords");
                    break;
                case "Head":
                    return RedirectToAction("Index", "OutstandingOrder");
                    break;
                case "Rep":
                    return RedirectToAction("Index", "Catalogues");
                    break;
                case "CoveringHead":
                    return RedirectToAction("Index", "OutstandingOrder");
                    break;
                case "Employee":
                    return RedirectToAction("Index", "NewRequest");
                    break;              
            }
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
