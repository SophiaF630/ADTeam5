﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    [Authorize]
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
            else
            {
                edit = false;
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
            DateTime dtpDefault = new DateTime(0001, 1, 1, 0, 0, 0);

            if (startdate != null && enddate != null)
            {
                if (startdate.Equals(dtpDefault) || enddate.Equals(dtpDefault))
                {
                    TempData["NoDetails"] = "Please fill in all details.";
                    @ViewData["Show"] = null;
                    return RedirectToAction("Index");
                }
                if (startdate <= enddate && startdate >= DateTime.Now.Date && enddate >= DateTime.Now.Date)
                {
                    if (ModelState.IsValid)
                    {
                        Models.Department d1 = context.Department.Where(x => x.DepartmentCode == dept).First();
                        d1.CoveringHeadId = u.UserId;

                        if (edit == true)
                        {
                            var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId).FirstOrDefault();
                            Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                            d2 = q;
                            d2.UserId = u.UserId;
                            d2.StartDate = startdate;
                            d2.EndDate = enddate;

                            context.SaveChanges();
                            TempData["EditSuccess"] = "Changes were saved successfully!";

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
                            context.SaveChanges();
                            TempData["NewSuccess"] = "New deputy head appointed!";

                            //send email to new deputy head
                            var newhead = context.User.Where(x => x.UserId == u.UserId).First();
                            string email = newhead.EmailAddress;
                            await _emailSender.SendEmailAsync(email, "Department Deputy Head Appointment", "Dear " + newhead.Name + ",<br>You have been appointed as the department deputy head.");
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["FilterError"] = "Assignment was not completed. Please try again.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (startdate > enddate && (startdate < DateTime.Now.Date || enddate < DateTime.Now.Date))
                    {
                        TempData["StartAndEndDateError"] = "End date cannot be earlier than start date. Start date and end date cannot be earlier than today. Please try again.";
                        return RedirectToAction("Index");
                    }
                    if (startdate > enddate)
                    {
                        TempData["EndDateError"] = "End date cannot be earlier than start date. Please try again.";
                        return RedirectToAction("Index");
                    }
                    if (startdate < DateTime.Now.Date || enddate < DateTime.Now.Date)
                    {
                        TempData["StartDateError"] = "Start date and end date cannot be earlier than today. Please try again.";
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

    }
}