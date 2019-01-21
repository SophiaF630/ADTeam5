using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
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
        static int currentDeputyHeadId;
        static bool edit = false;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        BizLogic b = new BizLogic();

        public AssignDeputyController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            Models.Department d1 = b.getDepartmentDetails(dept);
            if (d1.CoveringHeadId != null)
            {
                edit = true;
                currentDeputyHeadId = (int)d1.CoveringHeadId;
                var q2 = context.User.Where(x => x.UserId == currentDeputyHeadId).First();
                string name = q2.Name;
                ViewData["CurrentDeputyHead"] = name;
                DateTime today = DateTime.Now;
                var q3 = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId && x.EndDate >= today).First();
                DepartmentCoveringHeadRecord d2 = new DepartmentCoveringHeadRecord();
                d2 = q3;
                DateTime CurrentDeputyHeadStartDate = d2.StartDate;
                DateTime CurrentDeputyHeadEndDate = d2.EndDate;
                ViewData["CurrentDeputyHeadStartDate"] = CurrentDeputyHeadStartDate.ToShortDateString();
                ViewData["CurrentDeputyHeadEndDate"] = CurrentDeputyHeadEndDate.ToShortDateString();
            }

            List<User> u = new List<User>();

            Models.Department d = b.getDepartmentDetails(dept);
            int repid = d.RepId;
            int headid = d.HeadId;

            u = context.User.Where(x => x.DepartmentCode == dept && x.UserId != repid && x.UserId != headid).OrderBy(x => x.Name).ToList();
<<<<<<< HEAD
=======

>>>>>>> 08ac98e86165fd2d07115b3a2444bda46a155c1c
            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User u, DateTime startdate, DateTime enddate)
        {
            if (ModelState.IsValid)
            {
                int id = u.UserId;
                Models.Department d1 = context.Department.Where(x => x.DepartmentCode == dept).First();
                d1.CoveringHeadId = id;
                if (edit == true)
                {
                    var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId).First();
                    Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                    d2 = q;
                    d2.UserId = u.UserId;
                    d2.StartDate = startdate;
                    d2.EndDate = enddate;
                }
                else
                {
                    Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                    d2.UserId = u.UserId;
                    d2.StartDate = startdate;
                    d2.EndDate = enddate;
                    context.Add(d2);
                }

                context.SaveChanges();

                if(edit == true)
                {
                    TempData["Alert3"] = "Edits Saved Successfully";
                }
                else
                {
                    TempData["Alert1"] = "Deputy Head Appointed Successfully";
                }
                    return RedirectToAction("Index");
            }
<<<<<<< HEAD
            DateTime dt = DateTime.Now;
           
            if (startdate <dt)
=======
            else
>>>>>>> 08ac98e86165fd2d07115b3a2444bda46a155c1c
            {
                return RedirectToAction("Index");
            }
        }
    }
}
