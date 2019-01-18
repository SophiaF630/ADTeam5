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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ADTeam5.Areas.Identity.Data;

namespace ADTeam5.Controllers
{
    [Authorize]
    //this part should add in all controller
    public class DisbursementListsController : Controller
    {
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;

        BizLogic b = new BizLogic();

        readonly GeneralLogic userCheck;

        public DisbursementListsController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }
        // GET: DisbursementLists
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity =userCheck.checkUserIdentityAsync(user);
            ViewData["Department"] = identity[0];
            ViewData["role"] = identity[1];
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
            string depCode = _context.DisbursementList.Find(id).DepartmentCode;

            List<RecordDetails> rd = b.GenerateDisbursementListDetails(depCode);
            List<DisbursementListDetails> result = new List<DisbursementListDetails>();
            foreach (var item in rd)
            {
                DisbursementListDetails dlList = new DisbursementListDetails();

                dlList.ItemNumber = item.ItemNumber;
                //srList.ItemName = item.ItemNumberNavigation.ItemName;
                dlList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                dlList.Quantity = item.Quantity;
                dlList.Remark = item.Remark;

                result.Add(dlList);
            }
            return View(result);
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
            ViewData["RepId"] = new SelectList(_context.User, "UserId", "Name", disbursementList.RepId);

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
            //ViewData["RepId"] = new SelectList(_context.User, "UserId", "DepartmentCode", disbursementList.RepId);
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
            //ViewData["RepId"] = new SelectList(_context.User, "UserId", "DepartmentCode", disbursementList.RepId);
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
