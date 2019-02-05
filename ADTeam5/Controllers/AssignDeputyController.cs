﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class AssignDeputyController : Controller
    {
        static int userid;
        static string dept;
        static string role;
        static int currentDeputyHeadId;
        static bool edit = false;

        private readonly IEmailSender _emailSender;
        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        DeptBizLogic b = new DeptBizLogic();

        public AssignDeputyController(SSISTeam5Context context, UserManager<ADTeam5User> userManager, IEmailSender emailSender)
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
            if (d1.CoveringHeadId != null)
            {
                edit = true;
                currentDeputyHeadId = (int)d1.CoveringHeadId;
                string name = b.getCurrentDeputyHeadName(currentDeputyHeadId);
                ViewData["CurrentDeputyHead"] = name;
                Models.DepartmentCoveringHeadRecord d2 = b.findCurrentDeputyHeadToEdit(currentDeputyHeadId);
                ViewData["CurrentDeputyHeadStartDate"] = d2.StartDate.ToShortDateString();
                ViewData["CurrentDeputyHeadEndDate"] = d2.EndDate.ToShortDateString();
            }

            List<User> userList = new List<User>();
            Models.Department d = b.getDepartmentDetails(dept);
            int repid = d.RepId;
            int headid = d.HeadId;
            userList = b.populateAssignDeputyDropDownList(dept, repid, headid);
            ViewBag.listofitems = userList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User u, DateTime startdate, DateTime enddate)
        {
            if (startdate > enddate || startdate < DateTime.Now.Date.AddDays(-1))
            {
                TempData["DateAlert"] = "Please enter valid dates!";
                return RedirectToAction("Index");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Models.Department d1 = context.Department.Where(x => x.DepartmentCode == dept).First();
                    d1.CoveringHeadId = u.UserId;

                    if (edit == true)
                    {
                        var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId).First();
                        Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                        d2 = q;
                        d2.UserId = u.UserId;
                        d2.StartDate = startdate;
                        d2.EndDate = enddate;
                        //send email to old deputy head
                        var oldhead = context.User.Where(x => x.UserId == currentDeputyHeadId).First();
                        string email2 = oldhead.EmailAddress;
                        await _emailSender.SendEmailAsync(email2, "Department Deputy Head Replacement", "Dear " + oldhead.Name + ",<br>You have been replaced as department deputy head.");
                    }
                    else
                    {
                        Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                        d2.UserId = u.UserId;
                        d2.StartDate = startdate;
                        d2.EndDate = enddate;
                        context.Add(d2);
                    }
                    //send email to new deputy head
                    var newhead = context.User.Where(x => x.UserId == u.UserId).First();
                    string email = newhead.EmailAddress;
                    await _emailSender.SendEmailAsync(email, "Department Representative Appointment", "Dear " + newhead.Name + ",<br>You have been appointed as the department deputy head.");

                    //save changes
                    context.SaveChanges();
                    TempData["Success"] = "Edits Saved Successfully";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
        }

    }
}