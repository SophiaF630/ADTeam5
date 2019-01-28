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

namespace ADTeam5.Controllers
{
    public class AdjustmentRecordsController : Controller
    {

        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        public AdjustmentRecordsController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: AdjustmentRecords
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            //var sSISTeam5Context = _context.AdjustmentRecord.Include(a => a.clerkID == userID).Include(a => a.Manager).Include(a => a.Superviser);

            AdjustmentRecord ar = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp"));
            List<AdjustmentRecord> arList = new List<AdjustmentRecord>();
            if (ar != null)
            {
                arList = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp")).ToList();
            }
            else
            {
                NotFound();
            }
            return View(arList);
        }

        // GET: AdjustmentRecords/Details/5
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
            List<AdjustmentRecordDetails> result = new List<AdjustmentRecordDetails>();
            foreach (var item in rd)
            {
                AdjustmentRecordDetails arList = new AdjustmentRecordDetails();

                arList.ItemNumber = item.ItemNumber;
                arList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                arList.Quantity = item.Quantity;
                arList.Remark = item.Remark;

                result.Add(arList);
            }
            return View(result);
        }

        // POST: AdjustmentRecords/Details/5
        [HttpPost]
        public async Task<IActionResult> Details(string id, string itemNumber, int quantity, int rowID, string remark, int createNewVoucherItemModalName, int voucherItemModalName, string[] itemSubmitted, string[] itemSavedToDraft)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (id == null)
            {
                return NotFound();
            }

            
            
            //handle post action
            List<TempVoucherDetails> tempVoucherDetailsList = b.GetTempVoucherDetailsList(userID);

            if (createNewVoucherItemModalName == 1)
            {
                b.CreateNewVoucherItem(userID, itemNumber, quantity, remark);
                return RedirectToAction(nameof(Index));
            }
            else if (voucherItemModalName == 1)
            {
                b.UpdateVoucherItem(rowID, quantity, remark, tempVoucherDetailsList);
            }

            if (itemSubmitted.Length != 0)
            {
                //changestatus to pending approval
                //return RedirectToAction(nameof(Index));
            }
            else if (itemSavedToDraft.Length != 0)
            {
                //change status to draft
                //return RedirectToAction(nameof(Index));
            }

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;


            List<RecordDetails> rd = b.GetAdjustmentRecordDetails(id);
            List<AdjustmentRecordDetails> result = new List<AdjustmentRecordDetails>();
            foreach (var item in rd)
            {
                AdjustmentRecordDetails arList = new AdjustmentRecordDetails();

                arList.ItemNumber = item.ItemNumber;
                arList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                arList.Quantity = item.Quantity;
                arList.Remark = item.Remark;

                result.Add(arList);
            }
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> AdjustmentRecordSubmit(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            AdjustmentRecord adjustmentRecordToBeSubmitted = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
            adjustmentRecordToBeSubmitted.Status = "Submitted";
            _context.SaveChanges();

            AdjustmentRecord ar = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp"));
            List<AdjustmentRecord> tempAdjustmentRecords = new List<AdjustmentRecord>();
            if (ar != null)
            {
                tempAdjustmentRecords = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp")).ToList();
            }
            else
            {
                NotFound();
            }
            return PartialView("_TempAdjustmentRecords", tempAdjustmentRecords);
        }

        [HttpPost]
        public async Task<IActionResult> AdjustmentRecordDelete(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            
            AdjustmentRecord adjustmentRecordToBeDeleted = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
            _context.AdjustmentRecord.Remove(adjustmentRecordToBeDeleted);
            _context.SaveChanges();

            AdjustmentRecord ar = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp"));
            List<AdjustmentRecord> tempAdjustmentRecords = new List<AdjustmentRecord>();
            if (ar != null)
            {
                tempAdjustmentRecords = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp")).ToList();
            }
            else
            {
                NotFound();
            }
            return PartialView("_TempAdjustmentRecords", tempAdjustmentRecords);
        }

        [HttpPost]
        //[ActionName("VoucherItemDelete"), Route("~/IssueVoucher")]
        public async Task<IActionResult> VoucherItemDelete(int id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            List<TempVoucherDetails> tempVoucherDetailsList1 = b.GetTempVoucherDetailsList(userID);

            b.DeleteVoucherItem(id, tempVoucherDetailsList1);

            List<TempVoucherDetails> tempVoucherDetailsList = b.GetTempVoucherDetailsList(userID);

            if (tempVoucherDetailsList == null)
            {
                tempVoucherDetailsList = new List<TempVoucherDetails>();
            }
            return PartialView("_TempDetails", tempVoucherDetailsList);

        }

        //Create
        public async Task<IActionResult> Create()
        {
            return RedirectToAction("Index", "IssueVoucher", new { area = "" });
        }

        private bool AdjustmentRecordExists(string id)
        {
            return _context.AdjustmentRecord.Any(e => e.VoucherNo == id);
        }


       
    }
}
