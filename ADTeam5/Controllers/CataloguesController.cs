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

namespace ADTeam5.Controllers
{
    public class CataloguesController : Controller
    {

        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();

        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;

        public CataloguesController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
        }

        // GET: Catalogues
        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            if(dept == "STAS")
            {
                ViewData["ClerkRole"] = "true";
            }
            else
            {
                ViewData["ClerkRole"] = null;
            }

            var sSISTeam5Context = _context.Catalogue.Include(c => c.Supplier1Navigation).Include(c => c.Supplier2Navigation).Include(c => c.Supplier3Navigation);
            return View(await sSISTeam5Context.ToListAsync());
            
        }

        // GET: Catalogues/Details/5
        public async Task<IActionResult> Details(string id)
        {
            TempData["Delete"] = null;

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
            //ViewData["Supplier1"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode");
            //ViewData["Supplier2"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode" );
            //ViewData["Supplier3"] = new SelectList(_context.Supplier, "SupplierCode", "SupplierCode");

            //Viewbag for supplier1 dropdown list, need to post back
            List<Supplier> supplier1List = _context.Supplier.ToList();
            supplier1List.Insert(0, new Supplier { SupplierCode = "---Select Supplier---", SupplierName = "---Select Supplier---" });
            ViewBag.Supplier1 = supplier1List;
            List<Supplier> supplier2List = _context.Supplier.ToList();
            supplier2List.Insert(0, new Supplier { SupplierCode = "---Select Supplier---", SupplierName = "---Select Supplier---" });
            ViewBag.Supplier2 = supplier2List;
            List<Supplier> supplier3List = _context.Supplier.ToList();
            supplier3List.Insert(0, new Supplier { SupplierCode = "---Select Supplier---", SupplierName = "---Select Supplier---" });
            ViewBag.Supplier3 = supplier3List;

            return View(new Catalogue());
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemNumber,Category,ItemName,ReorderLevel,ReorderQty,UnitOfMeasure,Stock,Out,Supplier1,Supplier2,Supplier3,Supplier1Price,Supplier2Price,Supplier3Price")] Catalogue catalogue)
        {

            //Viewbag for supplier1 dropdown list, need to post back
            List<Supplier> supplier1List = _context.Supplier.ToList();
            supplier1List.Insert(0, new Supplier { SupplierCode = "---Select Supplier---", SupplierName = "---Select Supplier---" });
            ViewBag.Supplier1 = supplier1List;
            List<Supplier> supplier2List = _context.Supplier.ToList();
            supplier1List.Insert(0, new Supplier { SupplierCode = "---Select Supplier---", SupplierName = "---Select Supplier---" });
            ViewBag.Supplier2 = supplier2List;
            List<Supplier> supplier3List = _context.Supplier.ToList();
            supplier3List.Insert(0, new Supplier { SupplierCode = "---Select Supplier---", SupplierName = "---Select Supplier---" });
            ViewBag.Supplier3 = supplier3List;

            
            if (ModelState.IsValid)
            {
                _context.Add(catalogue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["EmptyError"] = "Please fill in all details!";
                return RedirectToAction("Create");
            }
            //return View(catalogue);
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

            var q = _context.RecordDetails.FirstOrDefault(x => x.ItemNumber == id);

            if (q == null)
            {
                ViewBag.IfCanDelete = "Y";
            }
            else
            {
                ViewBag.IfCanDelete = "N";
            }

            return View(catalogue);
        }

        // POST: Catalogues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var catalogue = await _context.Catalogue.FindAsync(id);
            //check whether the item is a FK of other records
            var q = _context.RecordDetails.FirstOrDefault(x => x.ItemNumber == id);
            
            if (q == null)
            {
                _context.Catalogue.Remove(catalogue);
                await _context.SaveChangesAsync();
                ViewBag.IfCanDelete = "Y";
                TempData["Delete"] = "Item is successfully deleted";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewBag.IfCanDelete = "N";
                return Redirect("Catalogue/Delete/" + id);
            }
            
        }

        private bool CatalogueExists(string id)
        {
            return _context.Catalogue.Any(e => e.ItemNumber == id);
        }

        public JsonResult GetSupplier1List(string supplier2, string supplier3)
        {

            List<Supplier> supplier1List = _context.Supplier.Where(x => x.SupplierCode != supplier2 && x.SupplierCode != supplier3).ToList();
            return Json(supplier1List);
        }

        public JsonResult GetSupplier2List(string supplier1, string supplier3)
        {

            List<Supplier> supplier2List = _context.Supplier.Where(x => x.SupplierCode != supplier1 && x.SupplierCode != supplier3).ToList();
            return Json(supplier2List);
        }

        public JsonResult GetSupplier3List(string supplier1, string supplier2)
        {
            List<Supplier> supplier3List = _context.Supplier.Where(x => x.SupplierCode != supplier1 && x.SupplierCode != supplier2).ToList();
            return Json(supplier3List);
        }
    }
}
