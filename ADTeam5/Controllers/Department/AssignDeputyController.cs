using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers.Department
{
    public class AssignDeputyController : Controller
    {
        private readonly SSISTeam5Context context;

        public AssignDeputyController(SSISTeam5Context context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            //Change Dept according to the person using this
            var q1 = context.Department.Where(x => x.DepartmentCode == "STAS" && x.CoveringHeadId != null).First();
            Models.Department d1 = q1;
            if (d1.CoveringHeadId!= null)
            {
                int coveringHeadId = (int)d1.CoveringHeadId;
                var q2 = context.User.Where(x => x.UserId == coveringHeadId).First();
                string name = q2.Name;
                ViewData["CurrentDeptHead"] = name;
            }

            List<User> u = new List<User>();

            var q = from x in context.Department where x.DepartmentCode == "STAS" select x;
            Models.Department d = q.First();
            int repid = d.RepId;
            int headid = d.HeadId;

            //Filter according to dept of the person who is using this
            u = context.User.Where(x => x.DepartmentCode == "STAS" && x.UserId != repid && x.UserId != headid).OrderBy(x => x.Name).ToList();
 
            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User u, DateTime startdate, DateTime enddate)
        {
            if(ModelState.IsValid)
            {
                int id = u.UserId;
                //Filter according to dept of the person who is using this
                Models.Department d1 = context.Department.Where(x => x.DepartmentCode == "STAS").First();
                d1.CoveringHeadId = id;

                Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                d2.UserId = u.UserId;
                d2.StartDate = startdate;
                d2.EndDate = enddate;
                context.Add(d2);

                context.SaveChanges();
                TempData["Alert1"] = "Deputy Head Appointed Successfully";
                return RedirectToAction("Index");
            }
            TempData["Alert2"] = "Please Try Again";
            return RedirectToAction("Index");

        }
    }
}