using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;

namespace ADTeam5.Controllers
{
    public class InventoryTransRecordsController : Controller
    {
        private readonly SSISTeam5Context _context;

        public InventoryTransRecordsController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: InventoryTransRecords
        public async Task<IActionResult> Index()
        {
            var sSISTeam5Context = _context.InventoryTransRecord.Include(i => i.ItemNumberNavigation);
            return View(await sSISTeam5Context.ToListAsync());
        }

        // GET: InventoryTransRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryTransRecord = await _context.InventoryTransRecord
                .Include(i => i.ItemNumberNavigation)
                .FirstOrDefaultAsync(m => m.TransId == id);
            if (inventoryTransRecord == null)
            {
                return NotFound();
            }

            return View(inventoryTransRecord);
        }

        // GET: InventoryTransRecords/Create
        public IActionResult Create()
        {
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber");
            return View();
        }

        // POST: InventoryTransRecords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransId,Date,ItemNumber,RecordId,Qty,Balance")] InventoryTransRecord inventoryTransRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventoryTransRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", inventoryTransRecord.ItemNumber);
            return View(inventoryTransRecord);
        }

        // GET: InventoryTransRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryTransRecord = await _context.InventoryTransRecord.FindAsync(id);
            if (inventoryTransRecord == null)
            {
                return NotFound();
            }
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", inventoryTransRecord.ItemNumber);
            return View(inventoryTransRecord);
        }

        // POST: InventoryTransRecords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransId,Date,ItemNumber,RecordId,Qty,Balance")] InventoryTransRecord inventoryTransRecord)
        {
            if (id != inventoryTransRecord.TransId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryTransRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryTransRecordExists(inventoryTransRecord.TransId))
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
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", inventoryTransRecord.ItemNumber);
            return View(inventoryTransRecord);
        }

        // GET: InventoryTransRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryTransRecord = await _context.InventoryTransRecord
                .Include(i => i.ItemNumberNavigation)
                .FirstOrDefaultAsync(m => m.TransId == id);
            if (inventoryTransRecord == null)
            {
                return NotFound();
            }

            return View(inventoryTransRecord);
        }

        // POST: InventoryTransRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventoryTransRecord = await _context.InventoryTransRecord.FindAsync(id);
            _context.InventoryTransRecord.Remove(inventoryTransRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryTransRecordExists(int id)
        {
            return _context.InventoryTransRecord.Any(e => e.TransId == id);
        }
    }
}
