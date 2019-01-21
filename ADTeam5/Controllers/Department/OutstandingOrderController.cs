using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
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
        BizLogic b = new BizLogic();

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

                return View(b.searchOutstandingRequests(dept));
            }

            // GET: OutstandingOrder/Details/id
            public IActionResult Details(string id)
            {
            rrid = id;

            ViewData["RRID"] = rrid;
            EmployeeRequestRecord e1 = b.searchEmployeeRequestByRRID(rrid);
            int userid = e1.DepEmpId;

            User u1 = b.getUser(userid);
            string username = u1.Name;
            ViewData["Name"] = username;

            return View(b.searchRecordDetailsByRRID(rrid));
            }

            [HttpPost]
            public IActionResult ApproveOrder()
            {
                EmployeeRequestRecord e1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
                e1.Status = "Approved";
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            //Please put in validation reason for null
            [HttpPost]
            public IActionResult RejectOrder(string rejectReason)
            {
                EmployeeRequestRecord e1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
                e1.Status = "Reject";
                e1.Remark = rejectReason.ToString();
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }
    }
