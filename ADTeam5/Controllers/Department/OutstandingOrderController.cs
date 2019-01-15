using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
{
    public class OutstandingOrderController : Controller
    {
            private readonly SSISTeam5Context context;
            private static string rrid;

            public OutstandingOrderController(SSISTeam5Context context)
            {
                this.context = context;
            }

            // GET: Outstanding Orders
            public IActionResult Index()
            {
                //Must filter by dept code based on dept head login
                var q = context.EmployeeRequestRecord.Where(x => x.Status == "Submitted");
                return View(q);
            }

            // GET: OutstandingOrder/Details/id
            public IActionResult Details(string id)
            {
                rrid = id;
                var q = context.RecordDetails.Where(x => x.Rrid == id);
                return View(q);
            }

            [HttpPost]
            public IActionResult ApproveOrder()
            {
                EmployeeRequestRecord e1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
                e1.Status = "Approved";
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            //Please put in validation reason for null
            [HttpPost]
            public IActionResult RejectOrder(string rejectReason)
            {
                EmployeeRequestRecord e1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
                e1.Status = "Reject";
                e1.Remark = rejectReason.ToString();
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
        }
    }
