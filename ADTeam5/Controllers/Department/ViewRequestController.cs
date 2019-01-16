using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
{
    public class ViewRequestController : Controller
    {
        private readonly SSISTeam5Context context;
        static string rrid;

        public ViewRequestController(SSISTeam5Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            //must filter by empId
            var q = context.EmployeeRequestRecord;
            return View(q);
        }

        public IActionResult Details(string id)
        {
            rrid = id;

            ViewData["RRID"] = rrid;
            var q1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e1 = q1;
            int userid = e1.DepEmpId;

            var q2 = context.User.Where(x => x.UserId == userid).First();
            User u1 = q2;
            string username = u1.Name;
            ViewData["Name"] = username;

            var q = context.RecordDetails.Where(x => x.Rrid == id);
            return View(q);
        }

    }
}
