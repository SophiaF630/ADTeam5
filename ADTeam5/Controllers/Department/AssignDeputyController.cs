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
                string name = b.getCurrentDeputyHeadName(currentDeputyHeadId);
                ViewData["CurrentDeputyHead"] = name;
                Models.DepartmentCoveringHeadRecord d2 = b.findCurrentDeputyHeadToEdit(currentDeputyHeadId);
                ViewData["CurrentDeputyHeadStartDate"] = d2.StartDate.ToShortDateString();
                ViewData["CurrentDeputyHeadEndDate"] = d2.EndDate.ToShortDateString();
            }
<<<<<<< HEAD
                List<User> userList = new List<User>();
                Models.Department d = b.getDepartmentDetails(dept);
                int repid = d.RepId;
                int headid = d.HeadId;
                userList = b.populateAssignDeputyDropDownList(dept, repid, headid);
                ViewBag.listofitems = userList;
                return View();
=======

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
>>>>>>> f6eb8b4697ba5f3bdb8083e6d9f8db26fae5cc4e
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
