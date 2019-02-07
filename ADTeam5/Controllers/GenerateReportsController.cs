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

        //Stationery usage
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
      
        //Data preparation
        public class ReportQueryData
        {
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public List<string> YearsName { get; set; }
            public List<string> MonthsName { get; set; }
            public List<string> Departments { get; set; }
            public List<string> Categories { get; set; }
        }

        public class DepartmentData
        {
            public string Name { get; set; }
            public string Stack { get; set; }
            public List<int> Data { get; set; }
        }

        public class ReportReturnData
        {
            public List<string> Xaxis { get; set; }
            public List<DepartmentData> Series { get; set; }
        }

        public List<DepartmentData> prepareData(List<string> departments, List<string> categories, List<StationeryUsageViewModel> rawData)
        {
            List<DepartmentData> reportData = new List<DepartmentData>();
            foreach (string dep in departments)
            {
                var deptName = _context.Department.Find(dep).DepartmentName;
                foreach (string cat in categories)
                {
                    DepartmentData depData = new DepartmentData();
                    depData.Name = dep + "/" + cat;
                    depData.Data = new List<int>();
                    depData.Stack = deptName;
                    var q = rawData.Where(x => x.DepCode == dep && x.Category == cat);
                    foreach(var item in q)
                    {
                        depData.Data.Add(item.QuantityDelivered);
                    }
                    reportData.Add(depData);
                }
                
            }
            return reportData;
        }

        public List<string> GetMonths(List<string> yearsName, List<string> monthsName, DateTime startDate, DateTime endDate)
        {
            List<string> months = new List<string>();
            if (yearsName == null || monthsName == null)
            {
                int startDateYear = startDate.Year;
                int startDateMonth = startDate.Month;
                int endDateYear = endDate.Year;
                int endDateMonth = endDate.Month;

                for (DateTime date = startDate; date <= endDate; date.AddMonths(1))
                {
                    string yearMonth = date.ToString("00") + "/" + date.ToString("0000");
                    months.Add(yearMonth);

                    if (startDate.Date > endDate.Date)
                    {
                        string endyearMonth = date.ToString("00") + "/" + date.ToString("0000");
                        months.Add(endyearMonth);
                    }
                }
                for (int i = 0; i < yearsName.Count(); i++)
                {
                    for (int j = 0; j < monthsName.Count(); j++)
                    {
                        string yearMonth = monthsName[j].PadLeft(2, '0') + "/" + yearsName[i];
                        months.Add(yearMonth);
                    }
                }
            }
            else if (yearsName != null && monthsName != null)
            {
                for (int i = 0; i < yearsName.Count(); i++)
                {
                    for (int j = 0; j < monthsName.Count(); j++)
                    {
                        string yearMonth = monthsName[j].PadLeft(2, '0') + "/" + yearsName[i];
                        months.Add(yearMonth);
                    }
                }
            }
            return months;
        }

        [HttpPost]
        public JsonResult GetStationeryUsageReportData(ReportQueryData queryData)
        {
            ReportReturnData reportData = new ReportReturnData();
            List<string> yearsName = queryData.YearsName;
            List<string> monthsName = queryData.MonthsName;
            DateTime startDate = Convert.ToDateTime(queryData.StartDate);
            DateTime endDate = Convert.ToDateTime(queryData.EndDate);
            List<string> departmentsCode = queryData.Departments;
            List<string> categoriesName = queryData.Categories;

            List<string> months = GetMonths(yearsName, monthsName, startDate, endDate);
            reportData.Xaxis = months;
            List<StationeryUsageViewModel> rawData = b.GetStationeryUsage("Completed", startDate, endDate, yearsName, monthsName, departmentsCode, categoriesName);

            reportData.Series = prepareData(departmentsCode, categoriesName, rawData);

            return Json(reportData);
        }



        //charge back
        public async Task<IActionResult> ChargeBack()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            List<ChargeBackViewModel> chargeBackViewModels = b.GetChargeBack("Completed");
            if (chargeBackViewModels.Count != 0)
            {
                //Viewbag for year and month dropdownlist, need to post back
                List<int> years = new List<int>();
                List<int> months = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                var q = chargeBackViewModels.GroupBy(x => new { x.Year }).Select(y => new { year = y.Key.Year });
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
                
            }
            return View();
        }

        //Data preparation
        public class ChargebackReportQueryData
        {
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public List<string> YearsName { get; set; }
            public List<string> MonthsName { get; set; }
            public List<string> Departments { get; set; }
        }

        public class ChargebackDepartmentData
        {
            public string Name { get; set; }
            public List<decimal?> Data { get; set; }
        }

        public class ChargebackReportReturnData
        {
            public List<string> Xaxis { get; set; }
            public List<ChargebackDepartmentData> Series { get; set; }
        }

        public List<ChargebackDepartmentData> ChargebackPrepareData(List<string> departments, List<ChargeBackViewModel> rawData)
        {
            List<ChargebackDepartmentData> reportData = new List<ChargebackDepartmentData>();
            foreach (string dep in departments)
            {
                var deptName = _context.Department.Find(dep).DepartmentName;
                ChargebackDepartmentData depData = new ChargebackDepartmentData();
                depData.Name = deptName;
                depData.Data = new List<decimal?>();
                var q = rawData.Where(x => x.DepCode == dep);
                foreach (var item in q)
                {
                    depData.Data.Add(item.TotalAmount);
                }
                reportData.Add(depData);
            }
            return reportData;
        }

        [HttpPost]
        public JsonResult GetChargebackReportData(ChargebackReportQueryData queryData)
        {
            ChargebackReportReturnData reportData = new ChargebackReportReturnData();
            List<string> yearsName = queryData.YearsName;
            List<string> monthsName = queryData.MonthsName;
            DateTime startDate = Convert.ToDateTime(queryData.StartDate);
            DateTime endDate = Convert.ToDateTime(queryData.EndDate);
            List<string> departmentsCode = queryData.Departments;

            List<string> months = GetMonths(yearsName, monthsName, startDate, endDate);
            reportData.Xaxis = months;
            List<ChargeBackViewModel> rawData = b.GetChargeBack("Completed", startDate, endDate, yearsName, monthsName, departmentsCode);

            reportData.Series = ChargebackPrepareData(departmentsCode, rawData);

            return Json(reportData);
        }
    }
}