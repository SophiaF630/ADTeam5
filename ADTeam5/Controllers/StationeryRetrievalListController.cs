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

namespace ADTeam5.Controllers
{
    public class StationeryRetrievalListController : Controller
    {
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();

        public StationeryRetrievalListController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: StationeryRetrievalList
        public async Task<IActionResult> Index()
        {
            //Generate disbursement list
            List<Department> dList = _context.Department.ToList();
            List<string> depCodeList = new List<string>();
            foreach (Department d in dList)
            {
                depCodeList.Add(d.DepartmentCode);
            }

            for (int i = 0; i < depCodeList.Count(); i++)
            {                
                //List<RecordDetails> rd = b.GenerateDisbursementListDetails(depCodeList[i]);
                List<RecordDetails> rd = b.GenerateDisbursementListDetails("ENGL");
            }

            List<StationeryRetrievalList> result = b.GetStationeryRetrievalLists();           
            return View(result);
        }

        // GET: StationeryRetrivalList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stationeryRetrivalList = await _context.RecordDetails
                .Include(r => r.ItemNumberNavigation)
                .FirstOrDefaultAsync(m => m.Rdid == id);
            if (stationeryRetrivalList == null)
            {
                return NotFound();
            }

            return View(stationeryRetrivalList);
        }

        //// GET: StationeryRetrivalList/Create
        //public IActionResult Create()
        //{
        //    ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber");
        //    return View();
        //}

        //// POST: StationeryRetrivalList/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Rdid,Rrid,ItemNumber,Quantity,Remark")] RecordDetails recordDetails)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(recordDetails);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", recordDetails.ItemNumber);
        //    return View(recordDetails);
        //}

        // GET: StationeryRetrivalList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recordDetails = await _context.RecordDetails.FindAsync(id);
            if (recordDetails == null)
            {
                return NotFound();
            }
            ViewData["ItemNumber"] = new SelectList(_context.Catalogue, "ItemNumber", "ItemNumber", recordDetails.ItemNumber);
            return View(recordDetails);
        }

        // POST: StationeryRetrivalList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Rdid,Rrid,ItemNumber,Quantity,Remark")] RecordDetails recordDetails)
        {
            if (id != recordDetails.Rdid)
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

        //// GET: StationeryRetrivalList/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var recordDetails = await _context.RecordDetails
        //        .Include(r => r.ItemNumberNavigation)
        //        .FirstOrDefaultAsync(m => m.Rdid == id);
        //    if (recordDetails == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(recordDetails);
        //}

        //// POST: StationeryRetrivalList/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var recordDetails = await _context.RecordDetails.FindAsync(id);
        //    _context.RecordDetails.Remove(recordDetails);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool RecordDetailsExists(int id)
        {
            return _context.RecordDetails.Any(e => e.Rdid == id);
        }
    }
}
