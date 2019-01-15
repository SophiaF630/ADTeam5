using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers.Department
{
    public class AssignDeputyController : Controller
    {
        private readonly SSISTeam5Context context;

        public AssignDeputyController(SSISTeam5Context context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            List<User> u = new List<User>();
            u = context.User.OrderBy(x => x.Name).ToList();
            ViewBag.listofitems = u;
            //ViewData["UserId"] = new SelectList(context.User, "UserId", "Name");
            return View();
        }
    
    }
}