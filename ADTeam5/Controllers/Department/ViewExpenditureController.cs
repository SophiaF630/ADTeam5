﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using ADTeam5.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;

namespace ADTeam5.Controllers.Department
{
    public class ViewExpenditureController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        static string dlid;
        decimal sum;

        DeptBizLogic b = new DeptBizLogic();

        public ViewExpenditureController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            List<DisbursementList> dbList = b.findDisbursementListStatusComplete(dept);
            decimal sum = b.findTotalExpenditure(dept, dbList);

            @ViewData["Sum"] = sum;
            return View(dbList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            ViewData["StartDate"] = startDate;
            ViewData["endDate"] = endDate;

    
            if (startDate != null && endDate != null)
            {
                if (ModelState.IsValid)
                {
                    List<DisbursementList> dbList = b.findDisbursementListStatusCompleteDateRange(dept, startDate, endDate);
                    decimal sum = b.findTotalExpenditure(dept, dbList);

                    @ViewData["Sum"] = sum;
                    return View(dbList);
                }

                TempData["Alert1"] = "Please fill in all details!";
                return RedirectToAction("Index"); 
            }
            else if (startDate > endDate || endDate < startDate)
            {
                //TempData["Alert1"] = "Please Try Again";
                //return RedirectToAction("Index");
                return Content("try again");
            }
            else 
            {
                var t = context.DisbursementList;
                return View(t);
            }

        }

        public IActionResult Details(string Dlid)
        {
            SSISTeam5Context e = new SSISTeam5Context();

            List<Renderview> rv = new List<Renderview>();
            rv = b.GetExpenditureDetails(Dlid);

            //var p = (from x in e.Catalogue
            //         join b in e.RecordDetails on x.ItemNumber equals b.ItemNumber
            //         join c in e.DisbursementList on b.Rrid equals c.Dlid
            //         where c.Dlid.Equals(Dlid) && c.Status.Equals("Completed")

            //         group new { x, b, c } by new { x.Category } into g

            //         select new Renderview

            //         {
            //             Category = g.Key.Category,
            //             Quantity = g.Sum(a => a.b.Quantity),
            //             Subtotal = g.Sum(a => a.x.Supplier1Price * a.b.Quantity)

            //         }).ToList();

            ViewBag.orderid = Dlid;
            return View(rv);
            
        }
    }
}