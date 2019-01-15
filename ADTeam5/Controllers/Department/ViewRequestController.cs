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

    }
}
