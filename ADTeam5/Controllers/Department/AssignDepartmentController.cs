﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ADTeam5.Controllers.Department
{
    public class AssignDepartmentController : Controller
    {
        private readonly SSISTeam5Context context;

        public AssignDepartmentController(SSISTeam5Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            List<User> u = new List<User>();
            u = context.User.OrderBy(x => x.Name).ToList();
            ViewBag.listofitems = u;
            //ViewData["UserId"] = new SelectList(context.User, "UserId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User u)
        {
            int id = u.UserId;
            var q = context.Department.Where(x => x.DepartmentCode == "STAS").First();
            if (q.RepId == id)
            {
                //ViewBag.IsValid = true;
                return Content("Same Person");
            }

            else
            {
                //ViewBag.IsValid = false;
                if (ModelState.IsValid)
                {
                    Models.Department d1 = context.Department.Where(x => x.DepartmentCode == "STAS").First();
                    d1.RepId = u.UserId;
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                return Content("Returned");
                //var selectedRep = u.Name;
                //return View(u);
            }
           
        }


    }
}