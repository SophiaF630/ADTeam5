using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Identity;
using ADTeam5.BusinessLogic;
using ADTeam5.Areas.Identity.Data;

namespace ADTeam5.Controllers
{
    public class InventoryTransRecordsController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        public InventoryTransRecordsController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: InventoryTransRecords
        public async Task<IActionResult> Index()
        {
            var inventoryTransRecord = _context.InventoryTransRecord.ToList();
            
            var result = new List<InventoryTransRecordDetails>();
            foreach (var item in inventoryTransRecord)
            {
                var inventoryTransRecordDetails = new InventoryTransRecordDetails();
                inventoryTransRecordDetails.TransId = item.TransId;
                inventoryTransRecordDetails.Date = item.Date;
                inventoryTransRecordDetails.ItemName = _context.Catalogue.Find(item.ItemNumber).ItemName;
                inventoryTransRecordDetails.DepartmentOrSupplier = b.FindDepartmentOrSupplier(item.RecordId);
                inventoryTransRecordDetails.Qty = item.Qty;
                inventoryTransRecordDetails.Balance = item.Balance;

                result.Add(inventoryTransRecordDetails);
            }
            return View(result);
        }

        // POST: InventoryTransRecords
        [HttpPost]
        public async Task<IActionResult> Index(DateTime startDate, DateTime endDate)
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
                        var t = _context.InventoryTransRecord.Where(s => s.Date >= startDate && s.Date <= endDate).OrderByDescending(x => x.TransId);
                        var result = new List<InventoryTransRecordDetails>();
                        foreach (var item in t)
                        {
                            var inventoryTransRecordDetails = new InventoryTransRecordDetails();
                            inventoryTransRecordDetails.TransId = item.TransId;
                            inventoryTransRecordDetails.Date = item.Date;
                            inventoryTransRecordDetails.ItemName = _context.Catalogue.Find(item.ItemNumber).ItemName;
                            inventoryTransRecordDetails.DepartmentOrSupplier = b.FindDepartmentOrSupplier(item.RecordId);
                            inventoryTransRecordDetails.Qty = item.Qty;
                            inventoryTransRecordDetails.Balance = item.Balance;

                            result.Add(inventoryTransRecordDetails);
                        }

                        return View(result);
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
    }
}
