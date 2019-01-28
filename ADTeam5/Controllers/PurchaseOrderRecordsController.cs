using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;
using ADTeam5.BusinessLogic;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.ViewModels;

namespace ADTeam5.Models
{

    public class PurchaseOrderRecordsController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        public PurchaseOrderRecordsController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: PurchaseOrderRecords
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            PurchaseOrderRecord purchaseOrderRecord = _context.PurchaseOrderRecord.FirstOrDefault(x => !x.Poid.Contains("POTemp"));
            List<PurchaseOrderRecord> poList = new List<PurchaseOrderRecord>();
            if (purchaseOrderRecord != null)
            {
                poList = _context.PurchaseOrderRecord.Where(x => !x.Poid.Contains("POTemp")).ToList();
            }
            else
            {
                NotFound();
            }
            return View(poList);
        }

        // GET: PurchaseOrderRecords/Details/5
        public async Task<IActionResult> Details(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (id == null)
            {
                return NotFound();
            }

            List<RecordDetails> rd = b.GetAdjustmentRecordDetails(id);
            List<PurchaseOrderRecordDetails> result = new List<PurchaseOrderRecordDetails>();
            foreach (var item in rd)
            {
                PurchaseOrderRecordDetails poList = new PurchaseOrderRecordDetails();

                poList.ItemNumber = item.ItemNumber;
                poList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                poList.Quantity = item.Quantity;

                result.Add(poList);
            }
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> PurchaseOrderRecordSubmit(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            PurchaseOrderRecord poRecordToBeSubmitted = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == id);
            poRecordToBeSubmitted.Status = "Submitted";
            _context.SaveChanges();

            PurchaseOrderRecord ar = _context.PurchaseOrderRecord.FirstOrDefault(x => !x.Poid.Contains("POTemp"));
            List<PurchaseOrderRecord> tempPurchaseOrderRecords = new List<PurchaseOrderRecord>();
            if (ar != null)
            {
                tempPurchaseOrderRecords = _context.PurchaseOrderRecord.Where(x => !x.Poid.Contains("POTemp")).ToList();
            }
            else
            {
                NotFound();
            }
            return PartialView("_TempPurchaseOrderRecords", tempPurchaseOrderRecords);
        }

        [HttpPost]
        public async Task<IActionResult> PurchaseOrderDelete(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            PurchaseOrderRecord poRecordToBeDeleted = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == id);
            _context.PurchaseOrderRecord.Remove(poRecordToBeDeleted);
            _context.SaveChanges();

            PurchaseOrderRecord ar = _context.PurchaseOrderRecord.FirstOrDefault(x => !x.Poid.Contains("POTemp"));
            List<PurchaseOrderRecord> tempPurchaseOrderRecords = new List<PurchaseOrderRecord>();
            if (ar != null)
            {
                tempPurchaseOrderRecords = _context.PurchaseOrderRecord.Where(x => !x.Poid.Contains("Vtemp")).ToList();
            }
            else
            {
                NotFound();
            }
            return PartialView("_TempPurchaseOrderRecords", tempPurchaseOrderRecords);
        }


        //Create
        public async Task<IActionResult> Create()
        {
            return RedirectToAction("Index", "RaisePurchaseOrder", new { area = "" });
        }

        private bool PurchaseOrderRecordExists(string id)
        {
            return _context.PurchaseOrderRecord.Any(e => e.Poid == id);
        }
    }
}
