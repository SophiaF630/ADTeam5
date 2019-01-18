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
            //Filter dept by the person who is using this
            var q1 = context.DisbursementList.Where(x => x.DepartmentCode == "ENGL" && x.Status == "Pending Delivery").First();
            DisbursementList d1 = q1;
            int currentCollectionPoint = d1.CollectionPointId;

            var q2 = context.CollectionPoint.Where(x => x.CollectionPointId == currentCollectionPoint).First();
            CollectionPoint c1 = q2;
            string currentName = c1.CollectionPointName;
            ViewData["Name"] = currentName;

            List < CollectionPoint > u = new List<CollectionPoint>();

            u = context.CollectionPoint.Where(x=> x.CollectionPointId != currentCollectionPoint).ToList();
            ViewBag.listofitems = u;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CollectionPoint cp)
        {
            //Filter by dept of person who is using this
            int c1 = cp.CollectionPointId;
            var q = context.DisbursementList.Where(x => x.DepartmentCode == "ENGL" && x.Status == "Pending Delivery").First();
            Models.DisbursementList d1 = q;
            d1.CollectionPointId = c1;
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
