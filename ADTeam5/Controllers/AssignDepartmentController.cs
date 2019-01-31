using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;

namespace ADTeam5.Controllers
{
    public class AssignDepartmentController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context context;
        DeptBizLogic b = new DeptBizLogic();
        readonly GeneralLogic userCheck;

        public AssignDepartmentController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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
            int currentRepId = d1.RepId;

            Models.User u1 = b.getUser(currentRepId);
            string currentRepName = u1.Name;

            ViewData["CurrentRepName"] = currentRepName;

            List<User> u = new List<User>();

            Models.Department d = b.getDepartmentDetails(dept);
            int repid = d.RepId;
            int headid = d.HeadId;
            int coverheadid = 0;
            if (d.CoveringHeadId != null)
            {
                coverheadid = (int)d.CoveringHeadId;
            }

            u = b.populateAssignDepartmentDropDownList(dept, repid, headid, coverheadid);

            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(User u)
        {
            if (ModelState.IsValid)
            {
                Department d2 = new Department();
                d2 = context.Department.Where(x => x.DepartmentCode == dept).First();
                d2.RepId = u.UserId;
                context.SaveChanges();
                TempData["Alert1"] = "Department Representative Changed Successfully";
                return RedirectToAction("Index");
            }
            TempData["Alert2"] = "Please Try Again";
            return RedirectToAction("Index");

        }

    }


}
