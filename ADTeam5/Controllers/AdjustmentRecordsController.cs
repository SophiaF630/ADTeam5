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
            var sSISTeam5Context = _context.AdjustmentRecord.Include(a => a.Clerk).Include(a => a.Manager).Include(a => a.Superviser);
            return View(await sSISTeam5Context.ToListAsync());
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

            List<RecordDetails> rd = b.GetAdjustmentRecordDetails(userID, id);
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

        // GET: AdjustmentRecords/Create
        public IActionResult Create()
        {
            ViewData["ClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode");
            ViewData["ManagerId"] = new SelectList(_context.User, "UserId", "DepartmentCode");
            ViewData["SuperviserId"] = new SelectList(_context.User, "UserId", "DepartmentCode");
            return View();
        }

        // POST: AdjustmentRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoucherNo,IssueDate,ApproveDate,ClerkId,SuperviserId,ManagerId,Status")] AdjustmentRecord adjustmentRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adjustmentRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.ClerkId);
            ViewData["ManagerId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.ManagerId);
            ViewData["SuperviserId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.SuperviserId);
            return View(adjustmentRecord);
        }

        // GET: AdjustmentRecords/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adjustmentRecord = await _context.AdjustmentRecord.FindAsync(id);
            if (adjustmentRecord == null)
            {
                return NotFound();
            }
            ViewData["ClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.ClerkId);
            ViewData["ManagerId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.ManagerId);
            ViewData["SuperviserId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.SuperviserId);
            return View(adjustmentRecord);
        }

        // POST: AdjustmentRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("VoucherNo,IssueDate,ApproveDate,ClerkId,SuperviserId,ManagerId,Status")] AdjustmentRecord adjustmentRecord)
        {
            if (id != adjustmentRecord.VoucherNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adjustmentRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdjustmentRecordExists(adjustmentRecord.VoucherNo))
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
            ViewData["ClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.ClerkId);
            ViewData["ManagerId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.ManagerId);
            ViewData["SuperviserId"] = new SelectList(_context.User, "UserId", "DepartmentCode", adjustmentRecord.SuperviserId);
            return View(adjustmentRecord);
        }

        // GET: AdjustmentRecords/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adjustmentRecord = await _context.AdjustmentRecord
                .Include(a => a.Clerk)
                .Include(a => a.Manager)
                .Include(a => a.Superviser)
                .FirstOrDefaultAsync(m => m.VoucherNo == id);
            if (adjustmentRecord == null)
            {
                return NotFound();
            }

            return View(adjustmentRecord);
        }

        // POST: AdjustmentRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var adjustmentRecord = await _context.AdjustmentRecord.FindAsync(id);
            _context.AdjustmentRecord.Remove(adjustmentRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdjustmentRecordExists(string id)
        {
            return _context.AdjustmentRecord.Any(e => e.VoucherNo == id);
        }
    }
}
