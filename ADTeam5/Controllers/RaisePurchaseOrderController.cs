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

        static string poNo = "";

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
            //b.AddReorderLevelItemToTempPurchaseOrderDetailsList();
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

        // POST: RaisePO
        [HttpPost]
        public async Task<IActionResult> Index(string itemNumber, int quantity, int rowID, string supplierCode, int createNewPOItemModalName, int POItemModalName, string[] itemSubmitted, string[] itemSavedToDraft)
        {
             ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
                List<string> identity = userCheck.checkUserIdentityAsync(user);
                int userID = user.WorkID;
                poNo = "";

                //handle post action
                List<TempPurchaseOrderDetails> tempPurchaseOrderDetailsList = b.GetTempPurchaseOrderDetailsList();

                if (createNewPOItemModalName == 1)
                {
                    b.CreateNewPOItem(userID, itemNumber, quantity, supplierCode);
                    return RedirectToAction(nameof(Index));
                }
                else if (POItemModalName == 1)
                {
                    b.UpdatePOItem(userID, rowID, quantity, supplierCode, tempPurchaseOrderDetailsList);
                }

                if (itemSubmitted.Length != 0)
                {
                    //split supplier
                    var supplier = tempPurchaseOrderDetailsList.GroupBy(x => x.SupplierCode).Select(y => y.Key);
                    foreach (var s in supplier)
                    {
                        poNo = b.IDGenerator("PO");
                        foreach (var item in tempPurchaseOrderDetailsList)
                        {
                            if (Array.Exists(itemSubmitted, i => i == item.RowID.ToString()) && item.SupplierCode == s)
                            {
                                b.AddItemsToPO(item.RowID, poNo, tempPurchaseOrderDetailsList);
                                b.CreatePurchaseOrderRecord(userID, poNo, s, "Pending Delivery");
                            }
                        }
                    }


                    //return RedirectToAction(nameof(Index));
                }
                else if (itemSavedToDraft.Length != 0)
                {
                    var supplier = tempPurchaseOrderDetailsList.GroupBy(x => x.SupplierCode).Select(y => y.Key);
                    foreach (var s in supplier)
                    {
                        poNo = b.IDGenerator("PO");
                        foreach (var item in tempPurchaseOrderDetailsList)
                        {
                            if (Array.Exists(itemSavedToDraft, i => i == item.RowID.ToString()))
                            {
                                b.AddItemsToPO(item.RowID, poNo, tempPurchaseOrderDetailsList);
                                b.CreatePurchaseOrderRecord(userID, poNo, s, "Draft");
                            }
                        }
                    }
                    //return RedirectToAction(nameof(Index));
                }

                List<TempPurchaseOrderDetails> result = b.GetTempPurchaseOrderDetailsList();

                //calculate order total amount by supplier


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

            List<TempPurchaseOrderDetails> tempPurchaseOrderDetailsList1 = b.GetTempPurchaseOrderDetailsList();

            b.DeletePOItem(id, tempPurchaseOrderDetailsList1);

            List<TempPurchaseOrderDetails> tempPurchaseOrderDetailsList = b.GetTempPurchaseOrderDetailsList();

            if (tempPurchaseOrderDetailsList == null)
            {
                tempPurchaseOrderDetailsList = new List<TempPurchaseOrderDetails>();
            }
            return PartialView("_TempPurchaseOrderDetailsList", tempPurchaseOrderDetailsList);

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
            string supplier1 = "";
            string supplier2 = "";
            string supplier3 = "";
            //Supplier supplier1 = new Supplier();
            //Supplier supplier2 = new Supplier();
            //Supplier supplier3 = new Supplier();
            //if (supplierCode1 != null)
            //{
            //    supplier1 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode1);
            //}
            //if (supplierCode2 != null)
            //{
            //    supplier2 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode2);
            //}
            //if (supplierCode3 != null)
            //{
            //    supplier3 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode3);
            //}
            if (supplierCode1 != null)
            {
                supplier1 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode1).SupplierName;
            }
            if (supplierCode2 != null)
            {
                supplier2 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode2).SupplierName;
            }
            if (supplierCode3 != null)
            {
                supplier3 = _context.Supplier.FirstOrDefault(y => y.SupplierCode == supplierCode3).SupplierName;
            }
            List<string> supplierList = new List<string>();


            // List<Supplier> supplierList = new List<Supplier>();
            //supplierList = _context.Supplier.Where(x => x.SupplierCode == supplierCode1 || x.SupplierCode == supplierCode2 || x.SupplierCode == supplierCode3).ToList();
            supplierList.Add(supplierCode1);
            supplierList.Add(supplierCode2);
            supplierList.Add(supplierCode3);
            return Json(supplierList);
        }
    }
}
