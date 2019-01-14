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
    public class CataloguesController : Controller
    {
        private readonly SSISTeam5Context _context;

        public CataloguesController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: Catalogues
        public async Task<IActionResult> Index()
        {
            var sSISTeam5Context = _context.Catalogue.Include(c => c.Supplier1Navigation).Include(c => c.Supplier2Navigation).Include(c => c.Supplier3Navigation);
            return View(await sSISTeam5Context.ToListAsync());
        }

        // GET: Catalogues/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogue = await _context.Catalogue
                .Include(c => c.Supplier1Navigation)
                .Include(c => c.Supplier2Navigation)
                .Include(c => c.Supplier3Navigation)
                .FirstOrDefaultAsync(m => m.ItemNumber == id);
            if (catalogue == null)
            {
                return NotFound();
            }

            return View(catalogue);
        }

        // GET: Catalogues/Create
        public IActionResult Create()
        {
            ViewData["Supplier1"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode");
            ViewData["Supplier2"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode");
            ViewData["Supplier3"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode");
            return View();
        }

        // POST: Catalogues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemNumber,Category,ItemName,ReorderLevel,ReorderQty,UnitOfMeasure,Stock,Supplier1,Supplier2,Supplier3,Supplier1Price,Supplier2Price,Supplier3Price,Location")] Catalogue catalogue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catalogue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Supplier1"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier1);
            ViewData["Supplier2"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier2);
            ViewData["Supplier3"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier3);
            return View(catalogue);
        }

        // GET: Catalogues/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogue = await _context.Catalogue.FindAsync(id);
            if (catalogue == null)
            {
                return NotFound();
            }
            ViewData["Supplier1"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier1);
            ViewData["Supplier2"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier2);
            ViewData["Supplier3"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier3);
            return View(catalogue);
        }

        // POST: Catalogues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ItemNumber,Category,ItemName,ReorderLevel,ReorderQty,UnitOfMeasure,Stock,Supplier1,Supplier2,Supplier3,Supplier1Price,Supplier2Price,Supplier3Price,Location")] Catalogue catalogue)
        {
            if (id != catalogue.ItemNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catalogue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatalogueExists(catalogue.ItemNumber))
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
            ViewData["Supplier1"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier1);
            ViewData["Supplier2"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier2);
            ViewData["Supplier3"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode", catalogue.Supplier3);
            return View(catalogue);
        }

        // GET: Catalogues/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catalogue = await _context.Catalogue
                .Include(c => c.Supplier1Navigation)
                .Include(c => c.Supplier2Navigation)
                .Include(c => c.Supplier3Navigation)
                .FirstOrDefaultAsync(m => m.ItemNumber == id);
            if (catalogue == null)
            {
                return NotFound();
            }

            return View(catalogue);
        }

        // POST: Catalogues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var catalogue = await _context.Catalogue.FindAsync(id);
            _context.Catalogue.Remove(catalogue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CatalogueExists(string id)
        {
            return _context.Catalogue.Any(e => e.ItemNumber == id);
        }
    }
}
