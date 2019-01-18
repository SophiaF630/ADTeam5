using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.Models;

namespace ADTeam5.Controllers.Department
{
    public class ViewExpenditureController : Controller
    {
        private readonly SSISTeam5Context context;
        static string dlid;
        decimal sum;

        public ViewExpenditureController(SSISTeam5Context context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            //Filter according to the dept of the person using this
            var q = context.DisbursementList.Where(x => x.Status == "Completed" && x.DepartmentCode == "STAS").ToList();
            List<DisbursementList> dList = q;
            List<RecordDetails> rList = new List<RecordDetails>();
            for (int i = 0; i < dList.Count; i++)
            {
                DisbursementList d = dList[i];
                string dId = d.Dlid;
                var q1 = context.RecordDetails.Where(x => x.Rrid == dId).ToList();
                foreach (RecordDetails current in q1)
                {
                    rList.Add(current);
                }
            }

            for (int j = 0; j < rList.Count; j++)
            {
                RecordDetails r = rList[j];
                string ItemNum = r.ItemNumber;
                var q2 = context.Catalogue.Where(x => x.ItemNumber == ItemNum).First();
                decimal price = (decimal)q2.Supplier1Price;
                int Quantity = r.Quantity;
                decimal total = price * Quantity;
                sum += total;

            }

            @ViewData["Sum"] = sum;
            return View(q);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            ViewData["StartDate"] = startDate;
            ViewData["endDate"] = endDate;

            if (startDate != null && endDate != null)
            {
                var t = context.DisbursementList.Where(s => s.StartDate >= startDate && s.CompleteDate <= endDate);
                return View(t);
            }
            else
            {
                var t = context.DisbursementList;
                return View(t);
            }

        }
    }
}
