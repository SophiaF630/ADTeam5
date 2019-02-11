using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    [Authorize]
    public class DepartmentRequestController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        static string rrid;

        public DepartmentRequestController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            var q = from x in context.EmployeeRequestRecord
                    join s in context.User on x.DepEmpId equals s.UserId
                    where x.DepCode.Equals(dept)
                    select new DepartmentRequest
                    {
                        Rrid = x.Rrid,
                        RequestDate = x.RequestDate,
                        EmployeeName = s.Name,
                        Status = x.Status
                    };

            List<DepartmentRequest> list = q.ToList();

            return View(list);
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
                        var t = from x in context.EmployeeRequestRecord
                                join s in context.User on x.DepEmpId equals s.UserId
                                where x.RequestDate >= startDate && x.RequestDate <= endDate && x.DepCode == dept
                                orderby x.Rrid descending
                                select new DepartmentRequest
                                {
                                    Rrid = x.Rrid,
                                    RequestDate = x.RequestDate,
                                    EmployeeName = s.Name,
                                    Status = x.Status
                                };

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
            var q3 = context.User.Where(x => x.UserId == e1.DepEmpId).FirstOrDefault();
            string name = q3.Name;

            DateTime requestDate = e1.RequestDate;
            ViewData["Status"] = e1.Status;
            ViewData["RequestDate"] = requestDate.ToShortDateString();
            ViewData["Requester"] = name;

            if (e1.Status == "Pending Approval")
            {
                return RedirectToAction("Edit", "ViewRequest", new { id });
            }
            else
            {
                var reject = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).FirstOrDefault();
                if (reject.Status == "Rejected")
                {
                    ViewData["RejectStatus"] = reject.Remark;
                }

                var q2 = from x in context.RecordDetails
                         join s in context.Catalogue on x.ItemNumber equals s.ItemNumber
                         where x.Rrid == rrid
                         select new ViewRequestDetails
                         {
                             itemName = s.ItemName,
                             quantity = x.Quantity
                         };
                List<ViewRequestDetails> list = new List<ViewRequestDetails>();
                list = q2.ToList();
                return View(list);
            }

        }

    }
}