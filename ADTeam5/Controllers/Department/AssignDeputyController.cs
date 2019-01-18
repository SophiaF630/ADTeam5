using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers.Department
{
    public class AssignDeputyController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;

        public AssignDeputyController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        public async Task<IActionResult>Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            var q1 = context.Department.Where(x => x.DepartmentCode == dept).First();
            Models.Department d1 = q1;
            if (d1.CoveringHeadId!= null)
            {
                int coveringHeadId = (int)d1.CoveringHeadId;
                var q2 = context.User.Where(x => x.UserId == coveringHeadId).First();
                string name = q2.Name;
                ViewData["CurrentDeptHead"] = name;
            }

            List<User> u = new List<User>();

            var q = from x in context.Department where x.DepartmentCode == dept select x;
            Models.Department d = q.First();
            int repid = d.RepId;
            int headid = d.HeadId;

            u = context.User.Where(x => x.DepartmentCode == dept && x.UserId != repid && x.UserId != headid).OrderBy(x => x.Name).ToList();
 
            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User u, DateTime startdate, DateTime enddate)
        {
            if(ModelState.IsValid)
            {
                int id = u.UserId;
                //Filter according to dept of the person who is using this
                Models.Department d1 = context.Department.Where(x => x.DepartmentCode == dept).First();
                d1.CoveringHeadId = id;

                Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                d2.UserId = u.UserId;
                d2.StartDate = startdate;
                d2.EndDate = enddate;
                context.Add(d2);

                await context.SaveChangesAsync();
                TempData["Alert1"] = "Deputy Head Appointed Successfully";
                return RedirectToAction("Index");
            }
            TempData["Alert2"] = "Please Try Again";
            return RedirectToAction("Index");

        }
    }
}