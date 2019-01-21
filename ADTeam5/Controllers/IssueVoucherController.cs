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
    public class IssueVoucherController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        static List<string> ItemNumberList = new List<string>();
        static List<int> QuantityList = new List<int>();

        public IssueVoucherController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
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

            List<TempVoucherDetails> tempVoucherDetailsList = b.GetTempVoucherDetailsList(userID);

            return View(tempVoucherDetailsList);
        }

        // POST: IssueVoucher
        [HttpPost]
        public async Task<IActionResult> Index(string itemNumber, int quantity)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            

            return View();
        }


        // GET: AdjustmentRecords/CreateVoucherItem
        public IActionResult Create()
        {
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber");
            //ViewData["ItemName"] = new SelectList(_context.Catalogue, "ItemName", "ItemName");
            return View();
        }

        // POST: AdjustmentRecords/CreateVoucherItem
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rdid,Rrid,ItemNumber,Quantity,Remark")] RecordDetails recordDetails)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (ModelState.IsValid)
            {
                recordDetails.Rrid = "VTemp" + userID.ToString();
                _context.Add(recordDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", recordDetails.ItemNumber);
            return View(recordDetails);
        }

        // GET: AdjustmentRecords/EditVoucherItem/5
        public async Task<IActionResult> Edit(string id)
        {

            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (id == null)
            {
                return NotFound();
            }
            string RRID = "VTemp" + userID.ToString();
            RecordDetails recordDetails = _context.RecordDetails.FirstOrDefault(x => x.Rrid == RRID && x.ItemNumber == id);

            if (recordDetails == null)
            {
                return NotFound();
            }
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", recordDetails.ItemNumber);
            return View(recordDetails);
        }

        // POST: AdjustmentRecords/EditVoucherItem/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Rdid,Rrid,ItemNumber,Quantity,Remark")] RecordDetails recordDetails)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (id != recordDetails.Rrid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recordDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecordDetailsExists(recordDetails.Rdid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", recordDetails.ItemNumber);
            return View(recordDetails);
        }

        [HttpPost]
        //[ActionName("VoucherItemDelete"), Route("~/IssueVoucher")]
        public async Task<IActionResult> VoucherItemDelete(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            string RRID = "VTemp" + userID.ToString();
            RecordDetails recordDetails = _context.RecordDetails.FirstOrDefault(x => x.Rrid == RRID && x.ItemNumber == id);
            _context.RecordDetails.Remove(recordDetails);
            _context.SaveChanges();
            List<TempVoucherDetails> tempVoucherDetailsList = b.GetTempVoucherDetailsList(userID);
            if(tempVoucherDetailsList == null)
            {
                tempVoucherDetailsList = new List<TempVoucherDetails>();
            }
            return PartialView("_TempVoucherDetailsList", tempVoucherDetailsList);
            //return View("Index", tempVoucherDetailsList);
        }

        

        private bool RecordDetailsExists(int id)
        {
            return _context.RecordDetails.Any(e => e.Rdid == id);
        }
    }
}
