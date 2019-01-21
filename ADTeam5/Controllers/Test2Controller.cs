using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class Test2Controller : Controller
    {
        public IActionResult Sidebar()
        {

            //ViewBag.itemname = new Bizlogic1().GetItemDescription;

            return View();
        }
    }
}