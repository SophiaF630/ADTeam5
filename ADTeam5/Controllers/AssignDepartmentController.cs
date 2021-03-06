﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ADTeam5.Controllers
{
    public class AssignDepartmentController : Controller
    {
        static int userid;
        static string dept;
        static string role;
        static int oldrepid;

        private readonly IEmailSender _emailSender;
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context context;
        DeptBizLogic b = new DeptBizLogic();
        readonly GeneralLogic userCheck;

        public AssignDepartmentController(SSISTeam5Context context, UserManager<ADTeam5User> userManager, IEmailSender emailSender)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            Models.Department d1 = b.getDepartmentDetails(dept);
            int currentRepId = d1.RepId;
            oldrepid = currentRepId;

            Models.User u1 = b.getUser(currentRepId);
            string currentRepName = u1.Name;

            ViewData["CurrentRepName"] = currentRepName;

            List<User> u = new List<User>();

            Models.Department d = b.getDepartmentDetails(dept);
            int repid = d.RepId;
            int headid = d.HeadId;
            int coverheadid = 0;
            if (d.CoveringHeadId != null)
            {
                coverheadid = (int)d.CoveringHeadId;
            }

            u = b.populateAssignDepartmentDropDownList(dept, repid, headid, coverheadid);

            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User u)
        {
            if (ModelState.IsValid)
            {
                int newUserId = u.UserId;
                Department d2 = new Department();
                d2 = context.Department.Where(x => x.DepartmentCode == dept).First();
                d2.RepId = newUserId;

                var t = context.DisbursementList.Where(x => x.DepartmentCode == dept && x.Status == "Pending Delivery");
                if (t.Any())
                {
                    DisbursementList d3 = new DisbursementList();
                       d3 =  t.First();
                    d3.RepId = newUserId;
                }

                context.SaveChanges();
                TempData["Alert1"] = "Department representative changed successfully!";

                //send email to new dept rep
                var q = context.User.Where(x => x.UserId == u.UserId).First();
                string email = q.EmailAddress;
                await _emailSender.SendEmailAsync(email, "Department Representative Appointment", "Dear " + q.Name + ",<br>You have been appointed as the department representative for stationery collection.");

                //send email to old dept rep
                var q2 = context.User.Where(x => x.UserId == oldrepid).First();
                string email2 = q2.EmailAddress;
                await _emailSender.SendEmailAsync(email2, "Department Representative Replacement", "Dear " + q2.Name + ",<br>You have been replaced as department representative.");

                return RedirectToAction("Index");
            }
            TempData["Alert2"] = "Please try again";
            return RedirectToAction("Index");

        }

    }


}
