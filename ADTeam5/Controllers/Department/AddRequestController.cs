using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers.Department
{
    public class AddRequestController : Controller
    {
        public IActionResult Sidebar()
        {
            ViewBag.itemname = new Bizlogic().GetItemDescription;
            return View();
        }
    }
}