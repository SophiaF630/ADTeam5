using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
{
    public class ViewRequestController : Controller
    {
        static int userid;
        static string dept;
        static string role;    

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        static string rrid;
        BizLogic b = new BizLogic();

        public ViewRequestController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            return View(b.searchRequestByDept(dept));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            ViewData["StartDate"] = startDate;
            ViewData["endDate"] = endDate;

            if (startDate != null && endDate != null)
            {
                if (ModelState.IsValid)
                {
                    return View(b.searchRequestByDateAndDept(startDate, endDate, dept));
                }
                else
                {
                    TempData["Alert2"] = "Please Fill in All Details!";
                    return RedirectToAction("Index");
                }
            }
            if (startDate > endDate || endDate < startDate)
            {
                //TempData["Alert1"] = "Start end error";
                //return RedirectToAction("Index");
                return Content("start end error");
            }
            else
            {
                return View(b.searchRequestByDept(dept));
            }

        }
        public IActionResult Details(string id)
        {
            rrid = id;
            ViewData["RRID"] = rrid;
            //var q1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e1 = new EmployeeRequestRecord();
            e1 = b.searchEmployeeRequestByRRID(rrid);

            if (e1.Status == "Submitted")
            {


                return RedirectToAction("Edit", "ViewRequest", new { id });



            }

            else
            {

                var q = context.RecordDetails.Where(x => x.Rrid == id);
                return View(q);

                //List<RecordDetails> rd1 = new List<RecordDetails>();
                //rd1 = b.searchRecordDetailsByRRID(rrid);
                //return View(rd1);

            }
      
            
        }

        public IActionResult Edit(string id)
        {
            rrid = id;
            ViewData["RRID"] = rrid;

            var q = context.RecordDetails.Where(x => x.Rrid == id).ToList();
            return View(q);


        }
    }
}

