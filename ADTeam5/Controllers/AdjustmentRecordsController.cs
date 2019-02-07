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

        //// GET: AdjustmentRecords
        //public async Task<IActionResult> Index()
        //{
        //    ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
        //    List<string> identity = userCheck.checkUserIdentityAsync(user);
        //    int userID = user.WorkID;
        //    string userRole = identity[1];

        //    List<AdjustmentRecord> arList = new List<AdjustmentRecord>();
        //    //records shown to clerk
        //    switch (userRole)
        //    {
        //        case "Clerk":
        //            AdjustmentRecord arC = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status == "Draft");
        //            if (arC != null)
        //            {
        //                arList = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status == "Draft").OrderByDescending(x => x.VoucherNo).ToList();
        //            }
        //            else
        //            {
        //                NotFound();
        //            }
        //            break;
        //        case "Supervisor":
        //            AdjustmentRecord arS = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Pending Approval" && !x.VoucherNo.Contains("Vtemp"));
        //            if (arS != null)
        //            {
        //                arList = _context.AdjustmentRecord.Where(x => x.Status == "Pending Approval" && !x.VoucherNo.Contains("Vtemp")).OrderByDescending(x => x.VoucherNo).ToList();
        //            }
        //            else
        //            {
        //                NotFound();
        //            }
        //            break;
        //        case "Manager":
        //            AdjustmentRecord arM = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Pending Manager Approval" && !x.VoucherNo.Contains("Vtemp"));
        //            if (arM != null)
        //            {
        //                arList = _context.AdjustmentRecord.Where(x => x.Status == "Pending Manager Approval" && !x.VoucherNo.Contains("Vtemp")).OrderByDescending(x => x.VoucherNo).ToList();
        //            }
        //            else
        //            {
        //                NotFound();
        //            }
        //            break;
        //    }
        //    return View(arList);
        //}

        // POST: AdjustmentRecords
        //[HttpPost]
        public async Task<IActionResult> Index(string state)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            string userRole = identity[1];

            List<AdjustmentRecord> arList = new List<AdjustmentRecord>();
            List<AdjustmentRecordViewModel> arViewModelList = new List<AdjustmentRecordViewModel>();

            switch (state)
            {
                case "OutstandingRecords":
                    //records shown to clerk
                    switch (userRole)
                    {
                        case "Clerk":
                            AdjustmentRecord arC = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status == "Draft");
                            if (arC != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && (x.Status == "Draft" || x.Status == "Rejected")).OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                        case "Supervisor":
                            AdjustmentRecord arS = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Pending Approval");
                            if (arS != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.Status == "Pending Approval").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                        case "Manager":
                            AdjustmentRecord arM = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Pending Manager Approval");
                            if (arM != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.Status == "Pending Manager Approval").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                    }
                    break;

                case "PastRecords":
                    //records shown to clerk
                    switch (userRole)
                    {
                        case "Clerk":
                            AdjustmentRecord arC = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status != "Draft");
                            if (arC != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status != "Draft" && x.Status != "Rejected").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                        case "Supervisor":
                            AdjustmentRecord arS = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Rejected" || x.Status == "Approved" || x.Status == "Pending Manager Approval");
                            if (arS != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.Status == "Rejected" || x.Status == "Approved").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                        case "Manager":
                            AdjustmentRecord arM = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Rejected" || x.Status == "Approved");
                            if (arM != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.Status == "Rejected" || x.Status == "Approved").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                    }
                    break;
                default:
                    switch (userRole)
                    {
                        case "Clerk":
                            AdjustmentRecord arC = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status == "Draft");
                            if (arC != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && (x.Status == "Draft" || x.Status == "Rejected")).OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                        case "Supervisor":
                            AdjustmentRecord arS = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Pending Approval");
                            if (arS != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.Status == "Pending Approval").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                        case "Manager":
                            AdjustmentRecord arM = _context.AdjustmentRecord.FirstOrDefault(x => x.Status == "Pending Manager Approval");
                            if (arM != null)
                            {
                                arList = _context.AdjustmentRecord.Where(x => x.Status == "Pending Manager Approval").OrderByDescending(x => x.VoucherNo).ToList();
                                arViewModelList = b.CreateAdjustmentRecordViewModel(arList);
                            }
                            else
                            {
                                NotFound();
                            }
                            break;
                    }
                    break;
            }
            
            return View(arViewModelList);
        }


        // GET: AdjustmentRecords/Details/5
        public async Task<IActionResult> Details(string id)
        {

            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            string userRole = identity[1];
            if (id == null)
            {
                return NotFound();
            }
            //ViewData for voucherNo
            ViewData["VoucherNo"] = id;

            //ViewBag for voucher price            
            decimal? amount = b.GetTotalAmountForVoucher(id);
            decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
            ViewBag.Amount = amount;
            ViewBag.GST = GST;
            ViewBag.TotalAmount = amount + GST;

            List<AdjustmentRecordDetails> result = b.GetAdjustmentRecordDetails(id);

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            //ViewBag for record status
            ViewBag.AdjustmentRecordStatus = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id).Status;

            return View(result);
        }

        // POST: AdjustmentRecords/Details/5
        [HttpPost]
        public async Task<IActionResult> Details(string id, string itemNumber, int quantity, int rowID, string remark, int createNewVoucherItemModalName, int voucherItemModalName, string itemSubmitted, string itemSavedToDraft, string rejectVoucher, string approveBySup, string approveByMan)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            string userRole = identity[1];
            if (id == null)
            {
                return NotFound();
            }

            //ViewBag for voucher price            
            decimal? amount = b.GetTotalAmountForVoucher(id);
            decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
            ViewBag.Amount = amount;
            ViewBag.GST = GST;
            ViewBag.TotalAmount = amount + GST;

            //ViewData for voucherNo
            ViewData["VoucherNo"] = id; 

            //handle post action
            List<AdjustmentRecordDetails> adjustmentRecordDetailsList = b.GetAdjustmentRecordDetails(id);

            if (createNewVoucherItemModalName == 1)
            {
                b.CreateNewVoucherItem(userID, id, itemNumber, quantity, remark);
                return RedirectToAction(nameof(Details));
            }
            else if (voucherItemModalName == 1)
            {
                b.UpdateVoucherItem(rowID, quantity, remark, adjustmentRecordDetailsList);
            }

            if (itemSubmitted == "1")
            {
                //changestatus to pending approval
                b.UpdateRecordStatus("Pending Approval", "AdjustmentRecord", id);
                return RedirectToAction(nameof(Index));
            }
            else if (itemSavedToDraft == "1")
            {
                //change status to draft
                b.UpdateRecordStatus("Draft", "AdjustmentRecord", id);
                return RedirectToAction(nameof(Index));
            }

            if (rejectVoucher == "1")
            {
                var ar = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
                if(ar != null)
                {
                    //changestatus to reject
                    b.RejectVoucher(userID, userRole, id);
                }
                return RedirectToAction(nameof(Index));
            }
            else if (approveBySup == "1")
            {
                //change status to draft
                b.ApproveVoucher(userID, userRole, id);
                return RedirectToAction(nameof(Index));
            }
            else if (approveByMan == "1")
            {
                //change status to draft
                b.ApproveVoucher(userID, userRole, id);
                return RedirectToAction(nameof(Index));
            }

            //Viewbag for category dropdown list, need to post back
            List <Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            //ViewBag for record status
            ViewBag.AdjustmentRecordStatus = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id).Status;

            List<AdjustmentRecordDetails> result = b.GetAdjustmentRecordDetails(id);

            return View(result);
        }


        //[HttpPost]
        //public async Task<IActionResult> AdjustmentRecordSubmit(string id)
        //{
        //    ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
        //    List<string> identity = userCheck.checkUserIdentityAsync(user);
        //    int userID = user.WorkID;

        //    AdjustmentRecord adjustmentRecordToBeSubmitted = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
        //    adjustmentRecordToBeSubmitted.Status = "Pending Approval";
        //    _context.SaveChanges();

        //    AdjustmentRecord ar = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp"));
        //    List<AdjustmentRecord> tempAdjustmentRecords = new List<AdjustmentRecord>();
        //    if (ar != null)
        //    {
        //        tempAdjustmentRecords = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp")).OrderByDescending(x => x.VoucherNo).ToList();
        //    }
        //    else
        //    {
        //        NotFound();
        //    }
        //    return PartialView("_TempAdjustmentRecords", tempAdjustmentRecords);
        //}

        [HttpPost]
        public async Task<IActionResult> AdjustmentRecordDelete(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            
            AdjustmentRecord adjustmentRecordToBeDeleted = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id);
            _context.AdjustmentRecord.Remove(adjustmentRecordToBeDeleted);
            _context.SaveChanges();
            b.RemoveRecordDetails(id);

            AdjustmentRecord ar = _context.AdjustmentRecord.FirstOrDefault(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp"));
            List<AdjustmentRecord> tempAdjustmentRecords = new List<AdjustmentRecord>();
            List<AdjustmentRecordViewModel> arViewModelList = new List<AdjustmentRecordViewModel>();
            if (ar != null)
            {
                tempAdjustmentRecords = _context.AdjustmentRecord.Where(x => x.ClerkId == userID && !x.VoucherNo.Contains("Vtemp") && x.Status == "Draft").OrderByDescending(x=>x.VoucherNo).ToList();
                arViewModelList = b.CreateAdjustmentRecordViewModel(tempAdjustmentRecords);
            }
            else
            {
                NotFound();
            }
            return PartialView("_TempAdjustmentRecords", arViewModelList);
        }

        [HttpPost]
        //[Route("AdjustmentRecords/Details/{id?}")]
        public async Task<IActionResult> VoucherItemDelete(string id, int rdid)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            //string voucherNo = _context.RecordDetails.FirstOrDefault(x => x.Rdid == rdid).Rrid;
            List<AdjustmentRecordDetails> adjustmentRecordDetailsList = new List<AdjustmentRecordDetails>();
            if (id != null)
            {
                List<AdjustmentRecordDetails> adjustmentRecordDetailsList1 = b.GetAdjustmentRecordDetails(id);
                b.DeleteVoucherItem(rdid, adjustmentRecordDetailsList1);
                adjustmentRecordDetailsList = b.GetAdjustmentRecordDetails(id);
            }

            if (adjustmentRecordDetailsList == null)
            {
                adjustmentRecordDetailsList = new List<AdjustmentRecordDetails>();
            }

            //ViewData for voucherNo
            ViewData["VoucherNo"] = id;

            //ViewBag for voucher price            
            decimal? amount = b.GetTotalAmountForVoucher(id);
            decimal? GST = Math.Round((decimal)(amount * (decimal?)0.07), 2);
            ViewBag.Amount = amount;
            ViewBag.GST = GST;
            ViewBag.TotalAmount = amount + GST;

            List<AdjustmentRecordDetails> result = b.GetAdjustmentRecordDetails(id);

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            //ViewBag for record status
            ViewBag.AdjustmentRecordStatus = _context.AdjustmentRecord.FirstOrDefault(x => x.VoucherNo == id).Status;

            return PartialView("_TempDetails", adjustmentRecordDetailsList);
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
