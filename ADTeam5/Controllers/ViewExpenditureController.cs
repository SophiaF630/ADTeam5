using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using ADTeam5.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;

namespace ADTeam5.Controllers
{
    public class ViewExpenditureController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        static string dlid;
        decimal sum;

        DeptBizLogic b = new DeptBizLogic();

        public ViewExpenditureController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            List<DisbursementList> dbList = new List<DisbursementList>();
            dbList = b.findDisbursementListStatusComplete(dept);
            decimal sum = b.findTotalExpenditure(dept, dbList);

            int todayYear = DateTime.Now.Year;
            int nextYear;
            int lastYear;
            int todayMonth = DateTime.Now.Month;

            if (todayMonth < 4)
            {
                lastYear = todayYear - 1;
                ViewData["startFinancialYear"] = "April " + lastYear;
                ViewData["endFinancialYear"] = "March " + todayYear;
            }
            else
            {
                nextYear = todayYear + 1;
                ViewData["startFinancialYear"] = "April " + todayYear;
                ViewData["endFinancialYear"] = "March" + nextYear;
            }
             
            @ViewData["Sum"] = sum;
            @ViewData["Show"] = "true";
            return View(dbList);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            ViewData["StartDate"] = startDate.ToShortDateString();
            ViewData["EndDate"] = endDate.ToShortDateString();

            DateTime dtpDefault = new DateTime(0001, 1, 1, 0, 0, 0);

            if (startDate != null && endDate != null)
            {
                if (startDate.Equals(dtpDefault) || endDate.Equals(dtpDefault))
                {
                    TempData["NoDetails"] = "Please fill in all search details.";
                    @ViewData["Show"] = null;
                    return RedirectToAction("Index");
                }
                if (startDate <= endDate && startDate <= DateTime.Now.Date && endDate <= DateTime.Now.Date)
                {
                    if (ModelState.IsValid)
                    {
                        List<DisbursementList> dbList = b.findDisbursementListStatusCompleteDateRange(dept, startDate, endDate);
                        decimal sum = b.findTotalExpenditure(dept, dbList);

                        @ViewData["Sum"] = sum;
                        return View(dbList);
                    }
                    else
                    {
                        TempData["FilterError"] = "Search request was not completed. Please try again.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (startDate > endDate && (startDate > DateTime.Now.Date || endDate > DateTime.Now.Date))
                    {
                        TempData["StartAndEndDateError"] = "End date cannot be earlier than start date. Start date and end date cannot be later than today. Please try again.";
                        return RedirectToAction("Index");
                    }
                    if (startDate > endDate)
                    {
                        TempData["EndDateError"] = "End date cannot be earlier than start date. Please try again.";
                        return RedirectToAction("Index");
                    }
                    if (startDate > DateTime.Now.Date || endDate > DateTime.Now.Date)
                    {
                        TempData["StartDateError"] = "Start date and end date cannot be later than today. Please try again.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["NoDetails"] = "Please fill in all details!";
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                TempData["NoDetails"] = "Please fill in all details!";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Details(string Dlid)
        {
            SSISTeam5Context e = new SSISTeam5Context();

            List<Renderview> rv = new List<Renderview>();
            rv = b.GetExpenditureDetails(Dlid);

            ViewBag.orderid = Dlid;
            return View(rv);
            
        }
    }
}