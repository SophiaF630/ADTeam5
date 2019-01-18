using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
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
            ViewData["StartDate"] = startDate;
            ViewData["endDate"] = endDate;

            if (startDate != null && endDate != null)
            {
                var t = context.EmployeeRequestRecord.Where(s => s.RequestDate >= startDate && s.RequestDate <= endDate && s.DepCode == dept);
                return View(t);
            }
            else
            {
                var t = context.EmployeeRequestRecord.Where(x => x.DepCode == dept);
                return View(t);
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
                var q = context.RecordDetails.Where(x => x.Rrid == id);
                return View(q);
            }
            else
            {
                return RedirectToAction("Edit");

            }
        }

        public IActionResult Edit(string id)
        {
            rrid = id;
            ViewData["RRID"] = rrid;
            var q = context.RecordDetails.Where(x => x.Rrid == id);
            return View(q);
        }
    }
}

