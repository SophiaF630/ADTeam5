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
            if (stationeryUsageViewModels.Count != 0)
            {
                //Viewbag for year and month dropdownlist, need to post back
                List<int> years = new List<int>();
                List<int> months = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                var q = stationeryUsageViewModels.GroupBy(x => new { x.Year }).Select(y => new { year = y.Key.Year });
                foreach (var item in q.ToList())
                {
                    years.Add(item.year);
                }
                ViewBag.ListOfYear = years;
                ViewBag.ListOfMonth = months;

                //Viewbag for department dropdownlist, need to post back   
                var departments = _context.Department.ToList();
                if (departments.Count != 0)
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
            }
            return View();
        }


        //[HttpPost]
        //public async Task<IActionResult> StationeryUsage(DateTime startDate, DateTime endDate, List<int> yearsName, List<int> monthsName, List<string> departmentsCode, List<string> categoriesName)
        //{
        //    //Validate start and end date
        //    //ViewData["StartDate"] = startDate;
        //    //ViewData["endDate"] = endDate;

        //    //if (startDate != null && endDate != null)
        //    //{
        //    //    if (ModelState.IsValid)
        //    //    {
        //    //        if (startDate > endDate || endDate < startDate)
        //    //        {
        //    //            TempData["Alert1"] = "Start end error";
        //    //            return RedirectToAction("StationeryUsage");

        //    //        }
        //    //        else
        //    //        {
        //    //            return View();
        //    //        }

        //    //    }
        //    //    else
        //    //    {
        //    //        TempData["Alert2"] = "Please Fill in All Details!";
        //    //        return RedirectToAction("StationeryUsage");
        //    //    }
        //    //}

        //    List<StationeryUsageViewModel> stationeryUsageViewModelsList = b.GetStationeryUsage("Completed", startDate, endDate, yearsName, monthsName, departmentsCode, categoriesName);

        //    return View(stationeryUsageViewModelsList);
        //}

        
        public JsonResult GetYearMonthList(DateTime startDate, DateTime endDate, string[] yearsName, string[] monthsName)
        {
            List<string> result = new List<string>();
           
            if (yearsName.Count() == 0 || monthsName.Count() == 0)
            {
                int startDateYear = startDate.Year;
                int startDateMonth = startDate.Month;
                int endDateYear = endDate.Year;
                int endDateMonth = endDate.Month;

                for (DateTime date = startDate; date <= endDate; date.AddMonths(1))
                {
                    string yearMonth = date.ToString("00") + "/" + date.ToString("0000");
                    result.Add(yearMonth);

                    if(startDate.Date > endDate.Date)
                    {
                        string endyearMonth = date.ToString("00") + "/" + date.ToString("0000");
                        result.Add(endyearMonth);
                    }
                }
                for (int i = 0; i < yearsName.Count(); i++)
                {
                    for (int j = 0; j < monthsName.Count(); j++)
                    {
                        string yearMonth = monthsName[j].PadLeft(2, '0') + "/" + yearsName[i];
                        result.Add(yearMonth);
                    }
                }
            }
            else if (yearsName.Count() != 0 && monthsName.Count() != 0)
            {
                for (int i = 0; i<yearsName.Count(); i++)
                {
                    for (int j=0; j<monthsName.Count(); j++)
                    {
                        string yearMonth = monthsName[j].PadLeft(2, '0') + "/" + yearsName[i];
                        result.Add(yearMonth);
                    }
                }
            }
            return Json(result);
        }
    }
}