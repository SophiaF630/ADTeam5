using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers
{
    public class ViewRequestController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        static string rrid;

        public ViewRequestController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            var q = context.EmployeeRequestRecord.Where(x => x.DepCode == dept);
            return View(q);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            ViewData["StartDate"] = startDate.ToShortDateString();
            ViewData["endDate"] = endDate.ToShortDateString();

            DateTime dtpDefault = new DateTime(0001, 1, 1, 0, 0, 0);

            if (startDate != null && endDate != null)
            {
                if (startDate.Equals(dtpDefault) || endDate.Equals(dtpDefault))
                {
                    TempData["NoDetails"] = "Please fill in all search details.";
                    return RedirectToAction("Index");
                }
                if (startDate <= endDate && startDate <= DateTime.Now.Date && endDate <= DateTime.Now.Date)
                {
                    if (ModelState.IsValid)
                    {
                        var t = context.EmployeeRequestRecord.Where(s => s.RequestDate >= startDate && s.RequestDate <= endDate && s.DepCode == dept);
                        return View(t);
                    }
                    else
                    {
                        TempData["FilterError"] = "Search request was not completed. Please try again.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (startDate > endDate && startDate > DateTime.Now.Date)
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
                    TempData["NoDetails"] = "Please fill in all search details.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["NoDetails"] = "Please fill in all search details.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Details(string id)
        {
            rrid = id;

            ViewData["RRID"] = rrid;
            var q1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e1 = q1;

            if (e1.Status == "Approved")
            {
                return RedirectToAction("Edit", "ViewRequest", new { id });
            }
            else
            {
                var q = context.RecordDetails.Where(x => x.Rrid == id);
                return View(q);
            }
        }
        public IActionResult Edit(string id)
        {
            rrid = id;
            ViewData["RRID"] = rrid;
            var q = context.RecordDetails.Where(x => x.Rrid == id);
            return View("Edit");
        }
    }
}

