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
        DeptBizLogic b = new DeptBizLogic();

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
        public IActionResult Index(User u, DateTime startdate, DateTime enddate)
        {
            DateTime dt = DateTime.Now;
            if (ModelState.IsValid)
            {
                int id = u.UserId;
                Models.Department d1 = b.getDepartmentDetails(dept);
                d1.CoveringHeadId = id;
                if (edit == true && startdate != null && enddate != null)
                {
                    var q = context.DepartmentCoveringHeadRecord.Where(x => x.UserId == currentDeputyHeadId).First();
                    Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                    d2 = q;
                    d2.UserId = u.UserId;
                    d2.StartDate = startdate;
                    d2.EndDate = enddate;
                    context.SaveChanges();
                }
                else
                {
                    Models.DepartmentCoveringHeadRecord d2 = new Models.DepartmentCoveringHeadRecord();
                    d2.UserId = u.UserId;
                    d2.StartDate = startdate;
                    d2.EndDate = enddate;
                    context.Add(d2);
                    context.SaveChanges();
                }


                if (startdate < dt || enddate < startdate)
                {
                    TempData["Alert2"] = "Please check your dates";
                }
                else if (edit == true && startdate >= dt)
                {
                    TempData["Alert3"] = "Edits Saved Successfully";
                }

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
