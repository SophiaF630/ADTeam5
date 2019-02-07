using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers
{
    public class ViewRequestController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;
        static string rrid;

        public ViewRequestController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            this.context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            var q = context.EmployeeRequestRecord.Where(x => x.DepCode == dept && x.DepEmpId == userid).OrderByDescending(x => x.Rrid);
            return View(q);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DateTime startDate, DateTime endDate)
        {
            ViewData["StartDate"] = startDate.ToShortDateString();
            ViewData["endDate"] = endDate.ToShortDateString();

            DateTime dtpDefault = new DateTime(0001, 1, 1, 0, 0, 0);

            if (startDate != null && endDate != null)
            {
                if (startDate.Equals(dtpDefault) || endDate.Equals(dtpDefault))
                {
                    TempData["NoDetails"] = "Please fill in all search details.";
                    return RedirectToAction("Index");
                }
                if (startDate <= endDate && startDate <= DateTime.Now.Date && endDate <= DateTime.Now.Date)
                {
                    if (ModelState.IsValid)
                    {
                        var t = context.EmployeeRequestRecord.Where(s => s.RequestDate >= startDate && s.RequestDate <= endDate && s.DepCode == dept && s.DepEmpId == userid).OrderByDescending(x => x.Rrid);
                        return View(t);
                    }
                    else
                    {
                        TempData["FilterError"] = "Search request was not completed. Please try again.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    if (startDate > endDate && (startDate > DateTime.Now.Date || endDate > DateTime.Now.Date))
                    {
                        TempData["StartAndEndDateError"] = "End date cannot be earlier than start date. Start date and end date cannot be later than today. Please try again.";
                        return RedirectToAction("Index");
                    }
                    if (startDate > endDate)
                    {
                        TempData["EndDateError"] = "End date cannot be earlier than start date. Please try again.";
                        return RedirectToAction("Index");
                    }
                    if (startDate > DateTime.Now.Date || endDate > DateTime.Now.Date)
                    {
                        TempData["StartDateError"] = "Start date and end date cannot be later than today. Please try again.";
                        return RedirectToAction("Index");
                    }
                    TempData["NoDetails"] = "Please fill in all search details.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["NoDetails"] = "Please fill in all search details.";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Details(string id)
        {
            rrid = id;

            ViewData["RRID"] = rrid;
            var q1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e1 = q1;

            ViewData["Status"] = e1.Status;

            if (e1.Status == "Pending Approval")
            {
                return RedirectToAction("Edit", "ViewRequest", new { id });
            }
            else
            {
                var reject = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).FirstOrDefault();
                if(reject.Status == "Rejected")
                {
                    ViewData["RejectStatus"] = reject.Remark; 
                }

                var q2 = from x in context.RecordDetails
                         join s in context.Catalogue on x.ItemNumber equals s.ItemNumber
                         where x.Rrid == rrid
                         select new ViewRequestDetails
                         {
                             itemName = s.ItemName,
                             quantity = x.Quantity
                         };
                List<ViewRequestDetails> list = new List<ViewRequestDetails>();
                list = q2.ToList();
                return View(list);
            }

        }
        public IActionResult Edit(string id)
        {

            List<Catalogue> categoryList = new List<Catalogue>();
            var q = context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            List<Catalogue> itemNameList = new List<Catalogue>();
            itemNameList = (from x in context.Catalogue select x).ToList();
            itemNameList.Insert(0, new Catalogue { ItemNumber = "0", ItemName = "---Select Item---" });
            ViewBag.ListofItemName = itemNameList;

            rrid = id;
            ViewData["RRID"] = rrid;

            var q1 = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).First();
            EmployeeRequestRecord e1 = q1;

            ViewData["Status"] = e1.Status;

            var check = context.RecordDetails.Where(x => x.Rrid == rrid);
            if(!check.Any())
            {
                ViewData["CheckRecords"] = null;

                return View();
            }
            else
            {
                ViewData["CheckRecords"] = "true";

                var q2 = from x in context.RecordDetails
                         join s in context.Catalogue on x.ItemNumber equals s.ItemNumber
                         where x.Rrid == rrid
                         select new ViewRequestDetails
                         {
                             rrid = x.Rrid,
                             itemName = s.ItemName,
                             quantity = x.Quantity
                         };
                List<ViewRequestDetails> list = new List<ViewRequestDetails>();
                list = q2.ToList();

                return View(list);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RequestItemDelete(string itemName, string rrid)
        {
            var q = context.Catalogue.Where(x => x.ItemName == itemName).FirstOrDefault();
            string itemNumber = q.ItemNumber;

            var q3 = context.RecordDetails.Where(x => x.Rrid == rrid && x.ItemNumber == itemNumber).FirstOrDefault();
            context.RecordDetails.Remove(q3);
            await context.SaveChangesAsync();

            return RedirectToAction("Edit", "ViewRequest", new { id = rrid });
        }
        [HttpPost]
        public async Task<IActionResult> RequestItemEdit(string itemName, int quantity)
        {
            var q = context.Catalogue.Where(x => x.ItemName == itemName).FirstOrDefault();
            string itemNumber = q.ItemNumber;
            var q3 = context.RecordDetails.Where(x => x.Rrid == rrid && x.ItemNumber == itemNumber).FirstOrDefault();
            q3.Quantity = quantity;
            await context.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = rrid });
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(string itemNumber, int quantity)
        {
            //var q = context.Catalogue.Where(x => x.ItemName == itemName).FirstOrDefault();
            //string itemNumber = q.ItemNumber;

            bool checkItemExists = false;
            var q = context.RecordDetails.Where(x => x.Rrid == rrid).ToList();
            foreach(RecordDetails current in q)
            {
                if(current.ItemNumber == itemNumber)
                {
                    checkItemExists = true;
                    current.Quantity = current.Quantity + quantity;
                    await context.SaveChangesAsync();
                    break;
                }
            }
            if(checkItemExists == false)
            {
                RecordDetails r = new RecordDetails();
                r.Rrid = rrid;
                r.ItemNumber = itemNumber;
                r.Quantity = quantity;
                context.Add(r);
                await context.SaveChangesAsync();
                return RedirectToAction("Edit", new { id = rrid });
            }
            else
            {

                return RedirectToAction("Edit", new { id = rrid });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRequest(string rrid)
        {
            List<RecordDetails> r = context.RecordDetails.Where(x => x.Rrid == rrid).ToList();
            foreach (RecordDetails current in r)
            {
                context.Remove(current);
            }
            EmployeeRequestRecord e = context.EmployeeRequestRecord.Where(x => x.Rrid == rrid).FirstOrDefault();
            context.Remove(e);

            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

