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

            //var q1 = context.DisbursementList.Where(x => x.DepartmentCode == "STAS" && x.Status == "PendingDelivery").First();
            //Models.DisbursementList d1 = q1;
            //int currentCollectionPoint = d1.CollectionPointId;

            //var q2 = context.CollectionPoint.Where(x => x.CollectionPointId == currentCollectionPoint).First();
            //CollectionPoint c1 = q2;
            //string currentName = c1.CollectionPointName;
            ViewData["Name"] = "currentName";

            List<CollectionPoint> u = new List<CollectionPoint>();
            u = context.CollectionPoint.ToList();
            ViewBag.listofitems = u;
            return View();
        }
    }
}
