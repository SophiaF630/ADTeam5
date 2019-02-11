using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using ADTeam5.BusinessLogic;
using ADTeam5.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ADTeam5.Controllers
{
    [Authorize]
    public class StationeryRetrievalListController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        public StationeryRetrievalListController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: StationeryRetrievalList
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

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

            List<StationeryRetrievalList> result = b.GetStationeryRetrievalLists();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string itemNumber, int quantityRetrieved, int quantityForVoucher, string remark, int quantityRetrievedModalName, int addToVoucherModalName)
        {

            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (itemNumber == null)
            {
                return NotFound();
            }

            if (addToVoucherModalName == 1)
            {

                if (quantityForVoucher == 0)
                {
                    TempData["QuantityError"] = "Please select a quantity to add to voucher. Quantity cannot be 0.";
                    return RedirectToAction("Index");
                }
                else
                {
                    b.CreateNewVoucherItem(userID, itemNumber, quantityForVoucher, remark);
                }
            }
            else if (quantityRetrievedModalName == 1)
            {
                var stockCheck = _context.Catalogue.Where(x => x.ItemNumber == itemNumber).FirstOrDefault();
                int stockAmt = stockCheck.Stock + stockCheck.Out;

                if (quantityRetrieved > stockAmt)
                {
                    TempData["InsufficientStock"] = "Stock level: " + stockAmt + ". There is insufficient stock. Please select a quantity less than or equals to stock level.";
                }
                else
                {
                    b.UpdateCatalogueOutAndStockAfterRetrieval(itemNumber, quantityRetrieved);
                }
            }

            List<StationeryRetrievalList> result = b.GetStationeryRetrievalLists();

            return View(result);

        }

        private bool RecordDetailsExists(int id)
        {
            return _context.RecordDetails.Any(e => e.Rdid == id);
        }
    }
}
