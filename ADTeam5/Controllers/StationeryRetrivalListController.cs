using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ADTeam5.Models;
using ADTeam5.ViewModels;

namespace ADTeam5.Controllers
{
    public class StationeryRetrivalListController : Controller
    {
        private readonly SSISTeam5Context _context;

        public StationeryRetrivalListController(SSISTeam5Context context)
        {
            _context = context;
        }

        // GET: StationeryRetrivalList
        public async Task<IActionResult> Index()
        {
            DateTime start = DateTime.Now;
            DateTime cutoff = DateTime.Now;
            if (start.DayOfWeek >= DayOfWeek.Thursday)
            {
                start = start.AddDays(-7);
            }
            while (start.DayOfWeek != DayOfWeek.Thursday)
            {
                start = start.AddDays(-1);
            }
            Console.WriteLine("{0}", start);

            if (cutoff.DayOfWeek >= DayOfWeek.Wednesday)
            {
                while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
                    cutoff = cutoff.AddDays(-1);
            }
            else
            {
                while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
                    cutoff = cutoff.AddDays(1);
            }
            Console.WriteLine("{0}", cutoff);

            var a = _context.EmployeeRequestRecord
                .Where(x => x.CompleteDate < start && x.CompleteDate < cutoff && x.Status == "Approved");
            var b = _context.DisbursementList
                .Where(x => x.Status == "PartialFullfilled");

            var q = from x in _context.RecordDetails
                    group x by x.ItemNumber into g
                    select new { ItemNumber = g.Key, QuantityNeeded = g.Sum(y => y.Quantity)};

            var p = from x in q
                    join y in _context.Catalogue on x.ItemNumber equals y.ItemNumber
                    select new { x.ItemNumber, y.ItemName, x.QuantityNeeded};

            List < StationeryRetrivalList > result = new List<StationeryRetrivalList>();       
            foreach(var item in p)
            {
                
                    StationeryRetrivalList srList = new StationeryRetrivalList();
                    srList.ItemNumber = item.ItemNumber;
                    srList.ItemName = item.ItemName;
                    srList.Quantity = item.QuantityNeeded;

                result.Add(srList);
            }

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
