﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using ADTeam5.BusinessLogic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;

namespace ADTeam5.Controllers
{
    [Authorize]
    //this part should add in all controller
    public class DisbursementListsController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;
        static List<DisbursementListDetails> tempDisbursementListDetails = new List<DisbursementListDetails>();


        public DisbursementListsController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }
        // GET: DisbursementLists
        public async Task<IActionResult> Index()
        {
            ViewData["InsufficientCheck"] = null;

            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            ViewData["Department"] = identity[0];
            ViewData["role"] = identity[1];

            //Generate disbursement list
            List<Models.Department> dList = _context.Department.ToList();
            List<string> depCodeList = new List<string>();
            foreach (Models.Department d in dList)
            {
                depCodeList.Add(d.DepartmentCode);
            }

            for (int i = 0; i < depCodeList.Count(); i++)
            {
                List<RecordDetails> rd = b.GenerateRecordDetailsOfDisbursementList(depCodeList[i]);
            }

            var sSISTeam5Context = _context.DisbursementList.Include(d => d.CollectionPointNavigation).Include(d => d.DepartmentCodeNavigation).Include(d => d.RepNavigation).OrderByDescending(x => x.Dlid);
            return View(await sSISTeam5Context.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string departmentName, DateTime estDeliverDate, int changeEstDeliverDateModalName)
        {

            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (departmentName == null)
            {
                return NotFound();
            }

            if (changeEstDeliverDateModalName == 1)
            {
                b.ChangeEstDeliverDate(departmentName, estDeliverDate);
            }
            var sSISTeam5Context = _context.DisbursementList.Include(d => d.CollectionPointNavigation).Include(d => d.DepartmentCodeNavigation).Include(d => d.RepNavigation).OrderByDescending(x => x.Dlid);

            return View(sSISTeam5Context);
        }


        // GET: DisbursementLists/Details/5
        public async Task<IActionResult> Details(string id)
        {
            
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            tempDisbursementListDetails = new List<DisbursementListDetails>();

            if (id == null)
            {
                return NotFound();
            }

            List<RecordDetails> rd = _context.RecordDetails.Where(x => x.Rrid == id).ToList();
            List<DisbursementListDetails> result = new List<DisbursementListDetails>();
            List<int> rowIDList = new List<int>();

            //viewbag for status check
            ViewBag.DisbursementListStatus = _context.DisbursementList.FirstOrDefault(x => x.Dlid == id).Status;

            //if pending delivery show temp disbursement list
            if (_context.DisbursementList.Find(id).Status == "Pending Delivery")
            {
                foreach (var q in tempDisbursementListDetails)
                {
                    rowIDList.Add(q.RowID);
                }
                int rowID = 1;
                foreach (var item in rd)
                {
                    if (item.Rrid == id)
                    {
                        DisbursementListDetails dlList = new DisbursementListDetails();
                        if (!rowIDList.Contains(rowID))
                        {
                            dlList.RowID = rowID;
                            dlList.RDID = item.Rdid;
                            dlList.ItemNumber = item.ItemNumber;
                            dlList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                            dlList.Quantity = item.Quantity;
                            dlList.QuantityDelivered = item.QuantityDelivered;
                            dlList.Remark = item.Remark;

                            tempDisbursementListDetails.Add(dlList);
                        }
                    }
                    rowID++;
                }
                return View(tempDisbursementListDetails);
            }
            else
            {
                foreach (var q in result)
                {
                    rowIDList.Add(q.RowID);
                }
                int rowID = 1;
                foreach (var item in rd)
                {
                    if (item.Rrid == id)
                    {
                        DisbursementListDetails dlList = new DisbursementListDetails();
                        if (!rowIDList.Contains(rowID))
                        {
                            dlList.RowID = rowID;
                            dlList.RDID = item.Rdid;
                            dlList.ItemNumber = item.ItemNumber;
                            dlList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                            dlList.Quantity = item.Quantity;
                            dlList.QuantityDelivered = item.QuantityDelivered;
                            dlList.Remark = item.Remark;

                            result.Add(dlList);
                        }
                    }
                    rowID++;
                }
                return View(result);
            }
        }

        // POST: DisbursementLists/Details/5
        [HttpPost]
        public async Task<IActionResult> Details(string id, int rowID, string itemNumber, int quantityDelivered, int quantityForVoucher, string remarkForDelivery, string remarkForVoucher, string confirmationPassword, int quantityDeliveredModalName, int addToVoucherModalName, int confirmDeliveryModalName, int noShowModalName, int backToListModalName)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            //viewbag for status check
            ViewBag.DisbursementListStatus = _context.DisbursementList.FirstOrDefault(x => x.Dlid == id).Status;

            if (addToVoucherModalName == 1)
            {
                if(quantityForVoucher == 0)
                {
                    TempData["QuantityError"] = "Please select a quantity to add to voucher. Quantity cannot be 0.";
                    return RedirectToAction("Details", new {id});
                }
                else
                {
                    b.CreateNewVoucherItem(userID, itemNumber, quantityForVoucher, remarkForVoucher);
                }
            }
            else if (quantityDeliveredModalName == 1)
            {
                var q = _context.RecordDetails.Where(x => x.Rrid == id && x.ItemNumber == itemNumber).FirstOrDefault();
                int quantityNeeded = q.Quantity;
                if(quantityDelivered > quantityNeeded)
                {
                    TempData["ExcessError"] = "Quantity delivered cannot be greater than quantity requested. Please try again.";
                    return RedirectToAction("Details", new { id });
                }
                if (quantityDelivered < quantityNeeded &&(remarkForDelivery == null || remarkForDelivery == ""))
                {
                    ViewData["InsufficientCheck"] = "true";
                    TempData["InsufficientError"] = "As quantity delivered is lesser than quantity requested, please key in remarks. Please try again.";
                    return RedirectToAction("Details", new { id });
                }
                foreach (DisbursementListDetails dlDetails in tempDisbursementListDetails)
                {
                    if (dlDetails.RowID == rowID)
                    {
                        dlDetails.QuantityDelivered = quantityDelivered;
                        dlDetails.Remark = remarkForDelivery;
                    }
                }
            }
            else if (confirmDeliveryModalName == 1)
            {
                string depCode = _context.DisbursementList.Find(id).DepartmentCode;
                string collectionPassword = _context.Department.Find(depCode).CollectionPassword;

                //check if password is correct
                if (confirmationPassword == collectionPassword)
                {

                    //update out quantity
                    string dlID = b.IDGenerator("DL");
                    foreach (var item in tempDisbursementListDetails)
                    {
                        string itemNo = item.ItemNumber;
                        int qtyDelivered = item.QuantityDelivered;
                        int rdid = item.RDID;
                        string remark = item.Remark;

                        //generate a disbursement list if partial fulfilled
                        if (qtyDelivered != item.Quantity)
                        {
                            int qty = item.Quantity - qtyDelivered;
                            b.GenerateDisbursementListForPartialFulfillment(itemNo, qty, remark, depCode, dlID);

                        }

                        b.UpdateCatalogueOutAfterDelivery(itemNo, qtyDelivered);
                        b.UpdateQuantityDeliveredAfterDelivery(qtyDelivered, rdid);

                        int balance = _context.Catalogue.Find(itemNo).Stock;
                        b.UpdateInventoryTransRecord(itemNo, id, -qtyDelivered, balance);
                    }
                    
                    //update disbursement list status
                    var disbursementList = _context.DisbursementList.Find(id);
                    disbursementList.Status = "Completed";
                    disbursementList.CompleteDate = DateTime.Now;
                    _context.DisbursementList.Update(disbursementList);
                    _context.SaveChanges();

                    TempData["CorrectPassword"] = "Successful confirmation. Delivery completed!";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //check
                    //show incorrect password
                    TempData["IncorrectPassword"] = "Incorrect collection password. Please try again.";
                }
            }
            else if(noShowModalName == 1)
            {
                //update disbursement list status
                var disbursementList = _context.DisbursementList.Find(id);
                disbursementList.Status = "No Show";
                disbursementList.CompleteDate = DateTime.Now;
                _context.DisbursementList.Update(disbursementList);
                _context.SaveChanges();

                //update out and stock
                var itemToReturn = _context.RecordDetails.Where(x => x.Rrid == id);
                if(itemToReturn != null)
                {
                    foreach(var item in itemToReturn.ToList())
                    {
                        int quantityRequested = item.Quantity;
                        var q = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber);
                        if(q != null)
                        {                            
                            int outQty = q.Out;
                            int preStock = q.Stock;
                            //out number cannot be negative
                            if(outQty >= quantityRequested)
                            {
                                q.Stock = preStock + quantityRequested;
                                q.Out = outQty - quantityRequested;
                            }
                            else
                            {
                                q.Stock = preStock + outQty;
                                q.Out = 0;
                            }
                           
                            _context.Catalogue.Update(q);
                            _context.SaveChanges();
                        }
                    }
                }
                
                return RedirectToAction(nameof(Index));
            }
            else if(backToListModalName == 1)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(tempDisbursementListDetails);
        }

        private bool DisbursementListExists(string id)
        {
            return _context.DisbursementList.Any(e => e.Dlid == id);
        }
    }
}
