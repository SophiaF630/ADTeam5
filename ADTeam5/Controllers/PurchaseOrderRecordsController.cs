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
        static List<PurchaseOrderRecordDetails> tempPurchaseOrderRecordDetails = new List<PurchaseOrderRecordDetails>();

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
                poList = _context.PurchaseOrderRecord.Where(x => !x.Poid.Contains("POTemp")).OrderByDescending(x =>x.Poid).ToList();
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

            tempPurchaseOrderRecordDetails = new List<PurchaseOrderRecordDetails>();

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            if (id == null)
            {
                return NotFound();
            }

            //ViewBag for voucher price            
            decimal? amount = b.GetTotalAmountForPO(id);
            decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
            ViewBag.Amount = amount;
            ViewBag.GST = GST;
            ViewBag.TotalAmount = amount + GST;
            ViewBag.POStatus = _context.PurchaseOrderRecord.Find(id).Status;


            tempPurchaseOrderRecordDetails = b.GetPurchaseOrderRecordDetails(id);
            return View(tempPurchaseOrderRecordDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Details(string id, int rowID, int quantity, int quantityDelivered, int POItemModalName, int POItemModalQtyDeliveredName, int confirmDeliveryModalName, int AddToDraftModalName, int SubmitModalName, int backToListModalName)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            ViewBag.POStatus = _context.PurchaseOrderRecord.Find(id).Status;
            //ViewBag for voucher price            
            decimal? amount = b.GetTotalAmountForPO(id);
            decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
            ViewBag.Amount = amount;
            ViewBag.GST = GST;
            ViewBag.TotalAmount = amount + GST;
            ViewBag.POStatus = _context.PurchaseOrderRecord.Find(id).Status;


            if (POItemModalName == 1)
            {
                foreach (var poDetails in tempPurchaseOrderRecordDetails)
                {
                    if (poDetails.RowID == rowID)
                    {
                        poDetails.Quantity = quantity;
                    }
                }
            }
            else if(AddToDraftModalName == 1)
            {
                //update quantity ordered
                foreach (var item in tempPurchaseOrderRecordDetails)
                {
                    string itemNo = item.ItemNumber;
                    int qty = item.Quantity;
                    int rdid = item.RDID;

                    b.UpdatePOItemQtyOrdered(rdid, qty);
                }
                //update disbursement list status
                var po = _context.PurchaseOrderRecord.Find(id);
                po.Status = "Draft";
                po.OrderDate = DateTime.Now;
                _context.PurchaseOrderRecord.Update(po);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));

            }
            else if (SubmitModalName == 1)
            {
                //update quantity ordered
                foreach (var item in tempPurchaseOrderRecordDetails)
                {
                    string itemNo = item.ItemNumber;
                    int qty = item.Quantity;
                    int rdid = item.RDID;

                    b.UpdatePOItemQtyOrdered(rdid, qty);
                }

                //update disbursement list status
                var po = _context.PurchaseOrderRecord.Find(id);
                po.Status = "Pending Delivery";
                po.OrderDate = DateTime.Now;
                _context.PurchaseOrderRecord.Update(po);
                _context.SaveChanges();

                return RedirectToAction("Details", id);
            }
            else if (POItemModalQtyDeliveredName == 1)
            {
                foreach (var poDetails in tempPurchaseOrderRecordDetails)
                {
                    if (poDetails.RowID == rowID)
                    {
                        poDetails.QuantityDelivered = quantityDelivered;
                    }
                }
            }
            else if (confirmDeliveryModalName == 1)
            {

                //update out quantity
                foreach (var item in tempPurchaseOrderRecordDetails)
                {
                    string itemNo = item.ItemNumber;
                    int qtyDelivered = item.QuantityDelivered;
                    int rdid = item.RDID;
                    
                    b.UpdateQuantityDeliveredAfterDelivery(qtyDelivered, rdid);
                    b.UpdateCatalogueStockAfterSupplierDelivery(itemNo, qtyDelivered);

                    int balance = _context.Catalogue.Find(itemNo).Stock;
                    b.UpdateInventoryTransRecord(itemNo, id, qtyDelivered, balance);
                }

                //update disbursement list status
                var po = _context.PurchaseOrderRecord.Find(id);
                    po.Status = "Completed";
                    po.CompleteDate = DateTime.Now;
                    _context.PurchaseOrderRecord.Update(po);
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
            }
            else if (backToListModalName == 1)
            {
                return RedirectToAction(nameof(Index));
            }

           

            return View("Details", tempPurchaseOrderRecordDetails);
        }


        //[HttpPost]
        //public async Task<IActionResult> PurchaseOrderRecordSubmit(string id)
        //{
        //    ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
        //    List<string> identity = userCheck.checkUserIdentityAsync(user);
        //    int userID = user.WorkID;

        //    PurchaseOrderRecord poRecordToBeSubmitted = _context.PurchaseOrderRecord.FirstOrDefault(x => x.Poid == id);
        //    poRecordToBeSubmitted.Status = "Submitted";
        //    _context.SaveChanges();

        //    PurchaseOrderRecord ar = _context.PurchaseOrderRecord.FirstOrDefault(x => !x.Poid.Contains("POTemp"));
        //    List<PurchaseOrderRecord> tempPurchaseOrderRecords = new List<PurchaseOrderRecord>();
        //    if (ar != null)
        //    {
        //        tempPurchaseOrderRecords = _context.PurchaseOrderRecord.Where(x => !x.Poid.Contains("POTemp")).ToList();
        //    }
        //    else
        //    {
        //        NotFound();
        //    }
        //    return PartialView("_TempPurchaseOrderRecords", tempPurchaseOrderRecords);
        //}

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
                tempPurchaseOrderRecords = _context.PurchaseOrderRecord.Where(x => !x.Poid.Contains("Vtemp")).OrderByDescending(x => x.Poid).ToList();
            }
            else
            {
                NotFound();
            }
            return PartialView("_TempPurchaseOrderRecords", tempPurchaseOrderRecords);
        }

        [HttpPost]
        public async Task<IActionResult> POItemDelete(string poid, int id)
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

            List<PurchaseOrderRecordDetails> purchaseOrderDetailsList1 = b.GetPurchaseOrderRecordDetails(poid);

            b.DeletePOItem(id, purchaseOrderDetailsList1);

            List<PurchaseOrderRecordDetails> purchaseOrderDetailsList = b.GetPurchaseOrderRecordDetails(poid);

            if (purchaseOrderDetailsList == null)
            {
                purchaseOrderDetailsList = new List<PurchaseOrderRecordDetails>();
            }
            return View("Details", purchaseOrderDetailsList);
        }

        

        private bool PurchaseOrderRecordExists(string id)
        {
            return _context.PurchaseOrderRecord.Any(e => e.Poid == id);
        }
    }
}
