using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ADTeam5.Models
{
    public class PurchaseOrderRecordsController : Controller
    {
        private readonly SSISTeam5Context _context;

        public PurchaseOrderRecordsController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: PurchaseOrderRecords
        public async Task<IActionResult> Index()
        {
            var sSISTeam5Context = _context.PurchaseOrderRecord.Include(p => p.StoreClerk).Include(p => p.SupplierCodeNavigation);
            return View(await sSISTeam5Context.ToListAsync());
        }

        // GET: PurchaseOrderRecords/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderRecord = await _context.PurchaseOrderRecord
                .Include(p => p.StoreClerk)
                .Include(p => p.SupplierCodeNavigation)
                .FirstOrDefaultAsync(m => m.Poid == id);
            if (purchaseOrderRecord == null)
            {
                return NotFound();
            }
            //this part is new added
            var details = from m in _context.RecordDetails select m;
            details = details.Where(s=>s.Rrid == purchaseOrderRecord.Poid);
            ViewData["RecordDetail"] = details;
            //new added finished
            return View(purchaseOrderRecord);
        }

        // GET: PurchaseOrderRecords/Create
        public IActionResult Create()
        {
            ViewData["StoreClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode");
            ViewData["SupplierCode"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode");
            return View();
        }

        // POST: PurchaseOrderRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Poid,OrderDate,ExpectedCompleteDate,CompleteDate,StoreClerkId,SupplierCode,Status")] PurchaseOrderRecord purchaseOrderRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrderRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode", purchaseOrderRecord.StoreClerkId);
            ViewData["SupplierCode"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", purchaseOrderRecord.SupplierCode);
            return View(purchaseOrderRecord);
        }

        // GET: PurchaseOrderRecords/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderRecord = await _context.PurchaseOrderRecord.FindAsync(id);
            if (purchaseOrderRecord == null)
            {
                return NotFound();
            }
            ViewData["StoreClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode", purchaseOrderRecord.StoreClerkId);
            ViewData["SupplierCode"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", purchaseOrderRecord.SupplierCode);
            return View(purchaseOrderRecord);
        }

        // POST: PurchaseOrderRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Poid,OrderDate,ExpectedCompleteDate,CompleteDate,StoreClerkId,SupplierCode,Status")] PurchaseOrderRecord purchaseOrderRecord)
        {
            if (id != purchaseOrderRecord.Poid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrderRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderRecordExists(purchaseOrderRecord.Poid))
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
            ViewData["StoreClerkId"] = new SelectList(_context.User, "UserId", "DepartmentCode", purchaseOrderRecord.StoreClerkId);
            ViewData["SupplierCode"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", purchaseOrderRecord.SupplierCode);
            return View(purchaseOrderRecord);
        }

        // GET: PurchaseOrderRecords/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrderRecord = await _context.PurchaseOrderRecord
                .Include(p => p.StoreClerk)
                .Include(p => p.SupplierCodeNavigation)
                .FirstOrDefaultAsync(m => m.Poid == id);
            if (purchaseOrderRecord == null)
            {
                return NotFound();
            }

            return View(purchaseOrderRecord);
        }

        // POST: PurchaseOrderRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var purchaseOrderRecord = await _context.PurchaseOrderRecord.FindAsync(id);
            _context.PurchaseOrderRecord.Remove(purchaseOrderRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderRecordExists(string id)
        {
            return _context.PurchaseOrderRecord.Any(e => e.Poid == id);
        }
    }
}
