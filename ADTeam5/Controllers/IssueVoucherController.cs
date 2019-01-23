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

            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select( x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "Select" });
            ViewBag.ListofCategory = categoryList;

            List<Catalogue> itemNameList = new List<Catalogue>();
            itemNameList = (from x in _context.Catalogue select x).ToList();
            itemNameList.Insert(0, new Catalogue { ItemNumber = "0", ItemName = "Select" });
            ViewBag.ListofItemName = itemNameList;

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
