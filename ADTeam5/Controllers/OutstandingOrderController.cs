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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers
{
    public class OutstandingOrderController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        private static string rrid;
        readonly GeneralLogic userCheck;
        DeptBizLogic b = new DeptBizLogic();

        public OutstandingOrderController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: Outstanding Orders
        public async Task<IActionResult>Index()
            {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            return View(b.getOutstandingOrders(dept));
            }

            // GET: OutstandingOrder/Details/id
            public IActionResult Details(string id)
            {
            rrid = id;

            ViewData["RRID"] = rrid;
            var q1 = b.findEmployeeRecord(rrid);
            EmployeeRequestRecord e1 = q1;
            int userid = e1.DepEmpId;

            var q2 = b.getUser(userid);
            User u1 = q2;
            string username = u1.Name;
            ViewData["Name"] = username;

            return View(b.getOutstandingOrdersDetails(rrid));
            }

            [HttpPost]
            public IActionResult ApproveOrder()
            {
                EmployeeRequestRecord e1 = b.findEmployeeRecord(rrid);
                e1.Status = "Approved";
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            //Please put in validation reason for null
            [HttpPost]
            public IActionResult RejectOrder(string rejectReason)
            {
                EmployeeRequestRecord e1 = b.findEmployeeRecord(rrid);
                e1.Status = "Reject";
                e1.Remark = rejectReason.ToString();
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }
    }
