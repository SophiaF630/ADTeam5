using System;
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
            //Change Dept according to the person using this
            var q1 = context.Department.Where(x => x.DepartmentCode == "STAS").First();
            Models.Department d1 = q1;
            int currentRepId = d1.RepId;

            var q2 = context.User.Where(x => x.UserId == currentRepId).First();
            Models.User u1 = q2;
            string currentRepName = u1.Name;

            ViewData["CurrentRepName"] = currentRepName;

            List<User> u = new List<User>();

            var q = from x in context.Department where x.DepartmentCode == "STAS" select x;
            Models.Department d = q.First();
            int repid = d.RepId;
            int headid = d.HeadId;
            int coverheadid = 0;
            if (d.CoveringHeadId != null)
            {
               coverheadid = (int)d.CoveringHeadId;
            }

            //Filter according to dept of the person who is using this
            u = context.User.Where(x => x.DepartmentCode == "STAS" && x.UserId != repid && x.UserId != headid && x.UserId != coverheadid).OrderBy(x => x.Name).ToList();

            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User u)
        {
                if (ModelState.IsValid)
                {
                    Models.Department d1 = context.Department.Where(x => x.DepartmentCode == "STAS").First();
                    d1.RepId = u.UserId;
                    context.SaveChanges();
                    TempData["Alert1"] = "Department Representative Changed Successfully";
                    return RedirectToAction("Index");
                }
                TempData["Alert2"] = "Please Try Again";
                return RedirectToAction("Index");

                //var selectedRep = u.Name;
                //return View(u);
            }
           
        }


    }
