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

            List<StationeryUsageViewModel> stationeryUsageViewModels = b.GetStationeryUsage("Completed");

            //Viewbag for year and month dropdownlist, need to post back
            List<int> years = new List<int>();
            List<int> months = new List<int>() { 1,2,3,4,5,6,7,8,9,10,11,12};
            var q = stationeryUsageViewModels.GroupBy(x => new { x.Year }).Select(y => new { year = y.Key.Year});
            foreach (var item in q.ToList())
            {
                years.Add(item.year);
            }
            ViewBag.ListOfYear = years;
            ViewBag.ListOfMonth = months;

            //Viewbag for department dropdownlist, need to post back   
            var departments = _context.Department.ToList();
            if(departments.Count != 0)
            {
                ViewBag.ListOfDepartment = departments.Select(x => new Department { DepartmentName = x.DepartmentName, DepartmentCode = x.DepartmentCode });
            }

            //Viewbag for category dropdownlist, need to post back   
            List<string> categories = new List<string>();
            var p = stationeryUsageViewModels.GroupBy(x => new { x.Category }).Select(y => new { category = y.Key.Category });
            foreach (var item in p.ToList())
            {
                categories.Add(item.category);
            }
            ViewBag.ListOfCategory = categories;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> StationeryUsage(string id)
        {
            return View();
        }
    }
}