using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.ViewModels;
using ADTeam5.BusinessLogic;
using Microsoft.AspNetCore.Identity;


namespace ADTeam5.Controllers
{
    public class RaisePurchaseOrderController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        static List<string> ItemNumberList = new List<string>();
        static List<int> QuantityList = new List<int>();
        static string voucherNo = "";

        public RaisePurchaseOrderController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: IssueVoucher
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            List<TempPurchaseOrderDetails> tempPurchaseOrderDetails = b.GetTempPurchaseOrderDetailsList();

            //Viewbag for catagory
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;
           

            return View(tempPurchaseOrderDetails);
        }

        // POST: IssueVoucher
        [HttpPost]
        public async Task<IActionResult> Index(string itemNumber, int quantity, int rowID, string remark, int createNewVoucherItemModalName, int voucherItemModalName, string[] itemSubmitted, string[] itemSavedToDraft)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            voucherNo = "";

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
                voucherNo = b.IDGenerator("V");
                foreach (var item in tempVoucherDetailsList)
                {
                    if (Array.Exists(itemSubmitted, i => i == item.RowID.ToString()))
                    {
                        b.AddItemsToVoucher(item.RowID, voucherNo, tempVoucherDetailsList);
                        b.CreateAdjustmentRecord(userID, voucherNo, "Submitted");
                    }
                }
                //return RedirectToAction(nameof(Index));
            }
            else if (itemSavedToDraft.Length != 0)
            {
                voucherNo = b.IDGenerator("V");
                foreach (var item in tempVoucherDetailsList)
                {
                    if (Array.Exists(itemSavedToDraft, i => i == item.RowID.ToString()))
                    {
                        b.AddItemsToVoucher(item.RowID, voucherNo, tempVoucherDetailsList);
                        b.CreateAdjustmentRecord(userID, voucherNo, "Draft");
                    }
                }
                //return RedirectToAction(nameof(Index));
            }

            List<TempVoucherDetails> result = b.GetTempVoucherDetailsList(userID);

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;
            return View(result);
        }


        [HttpPost]
        //[ActionName("VoucherItemDelete"), Route("~/IssueVoucher")]
        public async Task<IActionResult> POItemDelete(int id)
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
            return PartialView("_TempVoucherDetailsList", tempVoucherDetailsList);

        }

        private bool RecordDetailsExists(int id)
        {
            return _context.RecordDetails.Any(e => e.Rdid == id);
        }

        public JsonResult GetItemNameList(string category)
        {

            List<Catalogue> itemNameList = _context.Catalogue.Where(x => x.Category == category).ToList();
            return Json(itemNameList);
        }

        public JsonResult GetSuppliers(string itemNumber)
        {
            string supplierCode1 = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier1;
            string supplierCode2 = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier2;
            string supplierCode3 = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == itemNumber).Supplier3;
            Supplier supplier1 = new Supplier();
            Supplier supplier2 = new Supplier();
            Supplier supplier3 = new Supplier();
            if (supplierCode1 != null)
            {
                supplier1 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode1);
            }
            if (supplierCode2 != null)
            {
                supplier2 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode2);
            }
            if (supplierCode3 != null)
            {
                supplier3 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode3);
            }
            //List<string> supplierList = new List<string>();
            List<Supplier> supplierList = new List<Supplier>();
            //supplierList = _context.Supplier.Where(x => x.SupplierCode == supplierCode1 || x.SupplierCode == supplierCode2 || x.SupplierCode == supplierCode3).ToList();
            supplierList.Add(supplier1);
            supplierList.Add(supplier2);
            supplierList.Add(supplier3);
            return Json(supplierList);
        }
    }
}
