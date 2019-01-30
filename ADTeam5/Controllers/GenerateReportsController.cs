using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class GenerateReportsController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        public GenerateReportsController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        public async Task<IActionResult> StationeryUsage()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;



            List<StationeryUsageViewModel> completedDisbursementListDetails = b.GetStationeryUsage("Completed");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> StationeryUsage(string id)
        {
            return View();
        }
    }
}