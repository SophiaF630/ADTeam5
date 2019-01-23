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
    }
}
