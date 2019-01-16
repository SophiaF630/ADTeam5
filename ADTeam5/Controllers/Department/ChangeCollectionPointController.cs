using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers.Department
{
    public class ChangeCollectionPointController : Controller
    {
        private readonly SSISTeam5Context context;

        public ChangeCollectionPointController(SSISTeam5Context context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            List<CollectionPoint> u = new List<CollectionPoint>();
            u = context.CollectionPoint.ToList();            
            ViewBag.listofitems = u;
            return View();
        }
    }
}
