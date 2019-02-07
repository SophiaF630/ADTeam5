using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class ChangeCollectionPointController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly IEmailSender _emailSender;
        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        DeptBizLogic b = new DeptBizLogic();

        public ChangeCollectionPointController(SSISTeam5Context context, UserManager<ADTeam5User> userManager, IEmailSender emailSender)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            var q1 = b.getDepartmentDetails(dept);
            Department d = q1;
            int currentCollectionPoint = d.CollectionPointId;
            
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
                int newCollectionPoint = cp.CollectionPointId;
                var q1 = context.Department.Where(x => x.DepartmentCode == dept).First();
                Models.Department d2 = q1;
                d2.CollectionPointId = newCollectionPoint;

                TimeSpan cutoff = TimeSpan.Parse("17:30"); // 5.30 PM
                TimeSpan now = DateTime.Now.TimeOfDay;

                if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday || DateTime.Now.DayOfWeek == DayOfWeek.Wednesday || DateTime.Now.DayOfWeek == DayOfWeek.Thursday || (DateTime.Now.DayOfWeek == DayOfWeek.Friday && now <= cutoff))
                {
                    var t = context.DisbursementList.Where(x => x.DepartmentCode == dept && x.Status == "Pending Delivery");
                    if (t.Any())
                    {
                        DisbursementList dl = new DisbursementList();
                        dl = t.First();
                        dl.CollectionPointId = newCollectionPoint;
                    }
                }
                await context.SaveChangesAsync();
                TempData["Alert1"] = "Collection Point Changed Successfully";

                //send success email to dept rep
                var deptrep = context.User.Where(x => x.UserId == userid).First();
                string email = deptrep.EmailAddress;
                await _emailSender.SendEmailAsync(email, "Change in Stationery Collection Point", "Dear " + deptrep.Name + ",<br>Stationery collection point for " + deptrep.DepartmentCode + "department has successfully been changed.");
                
                return RedirectToAction("Index");
            }
            TempData["Alert2"] = "Please Try Again";
            return RedirectToAction("Index");
        }
    }
}
