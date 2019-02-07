using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers
{
    public class OutstandingOrderController : Controller
    {
        int userid;
        static string dept;
        static string role;

        private readonly IEmailSender _emailSender;
        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        private static string rrid;
        readonly GeneralLogic userCheck;
        DeptBizLogic b = new DeptBizLogic();

        public OutstandingOrderController(SSISTeam5Context context, UserManager<ADTeam5User> userManager, IEmailSender emailSender)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
            _emailSender = emailSender;
        }

        // GET: Outstanding Orders
        public async Task<IActionResult>Index()
            {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            List<OutstandingOrder> list = b.getOutstandingOrders(dept);
            if(list.Count == 0)
            {
                ViewData["Check"] = null;
                return View();
            }
            else
            {
                ViewData["Check"] = "true";
                return View(list);
            }
            
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
            public async Task<IActionResult> ApproveOrder()
            {
                EmployeeRequestRecord e1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            e1.Status = "Approved";
            DateTime today = DateTime.Now.Date;
            e1.CompleteDate = today;
                context.SaveChanges();
            int userid = e1.DepEmpId;

            //send success email to staff
            var approvedstaff = context.User.Where(x => x.UserId == userid).First();
            string email = approvedstaff.EmailAddress;
            await _emailSender.SendEmailAsync(email, "Approval of New Request", "Dear " + approvedstaff.Name + "<br>Your submitted stationery request has been rejected.");

            return RedirectToAction(nameof(Index));
            }

            //Please put in validation reason for null - Validated
            [HttpPost]
            public async Task<IActionResult> RejectOrder(string rejectReason)
            {
            if (String.IsNullOrEmpty(rejectReason))
            {
                TempData["NoText"] = "Please key in the reason for rejection.";
                return RedirectToAction(nameof(Details), new {id = rrid });
            }
            else
            {
                 EmployeeRequestRecord e1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
                e1.Status = "Rejected";
                e1.Remark = rejectReason.ToString();
                DateTime today = DateTime.Now.Date;
                e1.CompleteDate = today;
                context.SaveChanges();
                userid = e1.DepEmpId;

                //send rejection email to staff
                var rejectedstaff = context.User.Where(x => x.UserId == userid).First();
                string email = rejectedstaff.EmailAddress;
                await _emailSender.SendEmailAsync("Hello", "Rejection of New Request", "Dear,<br>Your submitted stationery request has been rejected.");

                return RedirectToAction(nameof(Index));
            }
            
        }
        }
    }
