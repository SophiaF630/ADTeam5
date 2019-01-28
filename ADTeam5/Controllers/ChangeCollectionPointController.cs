using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class ChangeCollectionPointController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        DeptBizLogic b = new DeptBizLogic();

        public ChangeCollectionPointController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            var q1 = b.findDisbursementListStatus(dept);
            DisbursementList d1 = q1;
            int currentCollectionPoint = d1.CollectionPointId;

            var q2 = b.findCollectionPointToEdit(currentCollectionPoint);
            CollectionPoint c1 = q2;
            string currentName = c1.CollectionPointName;
            ViewData["Name"] = currentName;

            List<CollectionPoint> u = new List<CollectionPoint>();

            u = context.CollectionPoint.Where(x => x.CollectionPointId != currentCollectionPoint).ToList();
            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CollectionPoint cp)
        {
            if (ModelState.IsValid)
            {
                int c1 = cp.CollectionPointId;
                var q = context.DisbursementList.Where(x => x.DepartmentCode == dept && x.Status == "Pending Delivery").First();
                Models.DisbursementList d1 = q;
                d1.CollectionPointId = c1;

                //int c2 = cp.CollectionPointId;
                var q1 = context.Department.Where(x => x.DepartmentCode == dept).First();
                Models.Department d2 = q1;
                d2.CollectionPointId = c1;

                await context.SaveChangesAsync();
                TempData["Alert1"] = "Collection Point Changed Successfully";
                return RedirectToAction("Index");
            }
            TempData["Alert2"] = "Please Try Again";
            return RedirectToAction("Index");


        }
    }
}
