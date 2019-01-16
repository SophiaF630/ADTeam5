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
    public class DisbursementListsController : Controller
    {
        private readonly SSISTeam5Context _context;

        public DisbursementListsController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: DisbursementLists
        public async Task<IActionResult> Index()
        {
            var sSISTeam5Context = _context.DisbursementList.Include(d => d.CollectionPointNavigation).Include(d => d.DepartmentCodeNavigation).Include(d => d.RepNavigation);
            return View(await sSISTeam5Context.ToListAsync());
        }

        // GET: DisbursementLists/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disbursementList = await _context.DisbursementList
                .Include(d => d.CollectionPointNavigation)
                .Include(d => d.DepartmentCodeNavigation)
                .Include(d => d.RepNavigation)
                .FirstOrDefaultAsync(m => m.Dlid == id);
            if (disbursementList == null)
            {
                return NotFound();
            }

            return View(disbursementList);
        }

        // GET: DisbursementLists/Create
        public IActionResult Create()
        {
            ViewData["CollectionPointId"] = new SelectList(_context.CollectionPoint, "CollectionPointId", "CollectionPointName");
            ViewData["DepartmentCode"] = new SelectList(_context.Department, "DepartmentCode", "DepartmentCode");
            ViewData["RepId"] = new SelectList(_context.User, "UserId", "DepartmentCode");
            return View();
        }

        // POST: DisbursementLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Dlid,StartDate,EstDeliverDate,CompleteDate,DepartmentCode,RepId,CollectionPointId,Status")] DisbursementList disbursementList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disbursementList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CollectionPointId"] = new SelectList(_context.CollectionPoint, "CollectionPointId", "CollectionPointName", disbursementList.CollectionPointId);
            ViewData["DepartmentCode"] = new SelectList(_context.Department, "DepartmentCode", "DepartmentCode", disbursementList.DepartmentCode);
            ViewData["RepId"] = new SelectList(_context.User, "UserId", "DepartmentCode", disbursementList.RepId);
            return View(disbursementList);
        }

        // GET: DisbursementLists/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disbursementList = await _context.DisbursementList.FindAsync(id);
            if (disbursementList == null)
            {
                return NotFound();
            }
            ViewData["CollectionPointId"] = new SelectList(_context.CollectionPoint, "CollectionPointId", "CollectionPointName", disbursementList.CollectionPointId);
            ViewData["DepartmentCode"] = new SelectList(_context.Department, "DepartmentCode", "DepartmentCode", disbursementList.DepartmentCode);
            ViewData["RepId"] = new SelectList(_context.User, "UserId", "DepartmentCode", disbursementList.RepId);
            return View(disbursementList);
        }

        // POST: DisbursementLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Dlid,StartDate,EstDeliverDate,CompleteDate,DepartmentCode,RepId,CollectionPointId,Status")] DisbursementList disbursementList)
        {
            if (id != disbursementList.Dlid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disbursementList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisbursementListExists(disbursementList.Dlid))
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
            ViewData["CollectionPointId"] = new SelectList(_context.CollectionPoint, "CollectionPointId", "CollectionPointName", disbursementList.CollectionPointId);
            ViewData["DepartmentCode"] = new SelectList(_context.Department, "DepartmentCode", "DepartmentCode", disbursementList.DepartmentCode);
            ViewData["RepId"] = new SelectList(_context.User, "UserId", "DepartmentCode", disbursementList.RepId);
            return View(disbursementList);
        }

        // GET: DisbursementLists/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disbursementList = await _context.DisbursementList
                .Include(d => d.CollectionPointNavigation)
                .Include(d => d.DepartmentCodeNavigation)
                .Include(d => d.RepNavigation)
                .FirstOrDefaultAsync(m => m.Dlid == id);
            if (disbursementList == null)
            {
                return NotFound();
            }

            return View(disbursementList);
        }

        // POST: DisbursementLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var disbursementList = await _context.DisbursementList.FindAsync(id);
            _context.DisbursementList.Remove(disbursementList);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DisbursementListExists(string id)
        {
            return _context.DisbursementList.Any(e => e.Dlid == id);
        }
    }
}
