using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
{
    public class ViewRequestController : Controller
    {
        private readonly SSISTeam5Context context;
        static string rrid;

        public ViewRequestController(SSISTeam5Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            //must filter by empId and make sure that DL and PO do not show
            var q = context.EmployeeRequestRecord;
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
                var t = context.EmployeeRequestRecord.Where(s => s.RequestDate >= startDate && s.RequestDate <= endDate);
                return View(t);
            }
            else
            {
                var t = context.EmployeeRequestRecord;
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

