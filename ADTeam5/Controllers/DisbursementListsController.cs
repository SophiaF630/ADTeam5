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
        static List<DisbursementListDetails> tempDisbursementListDetails = new List<DisbursementListDetails>();

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
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            ViewData["Department"] = identity[0];
            ViewData["role"] = identity[1];

            //ViewData["Department"] = identity[0];
            //ViewData["role"] = identity[1];

            //Generate disbursement list
            List<Models.Department> dList = _context.Department.ToList();
            List<string> depCodeList = new List<string>();
            foreach (Models.Department d in dList)
            {
                depCodeList.Add(d.DepartmentCode);
            }

            for (int i = 0; i < depCodeList.Count(); i++)
            {
                List<RecordDetails> rd = b.GenerateDisbursementListDetails(depCodeList[i]);
            }

            var sSISTeam5Context = _context.DisbursementList.Include(d => d.CollectionPointNavigation).Include(d => d.DepartmentCodeNavigation).Include(d => d.RepNavigation);
            return View(await sSISTeam5Context.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string departmentName, DateTime estDeliverDate, int changeEstDeliverDateModalName)
        {

            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (departmentName == null)
            {
                return NotFound();
            }

            if (changeEstDeliverDateModalName == 1)
            {
                b.ChangeEstDeliverDate(departmentName, estDeliverDate);
            }
            
            var sSISTeam5Context = _context.DisbursementList.Include(d => d.CollectionPointNavigation).Include(d => d.DepartmentCodeNavigation).Include(d => d.RepNavigation);

            return View(sSISTeam5Context);
        }


        // GET: DisbursementLists/Details/5
        public async Task<IActionResult> Details(string id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            if (id == null)
            {
                return NotFound();
            }

            List<RecordDetails> rd = _context.RecordDetails.Where(x => x.Rrid == id).ToList();
            List<DisbursementListDetails> result = new List<DisbursementListDetails>();
            foreach (var item in rd)
            {
                DisbursementListDetails dlList = new DisbursementListDetails();

                dlList.ItemNumber = item.ItemNumber;
                dlList.ItemName = _context.Catalogue.FirstOrDefault(x => x.ItemNumber == item.ItemNumber).ItemName;
                dlList.Quantity = item.Quantity;
                dlList.QuantityDelivered = 0;
                dlList.Remark = item.Remark;

                result.Add(dlList);
            }
            return View(result);
        }

        //// POST: DisbursementLists/Details/5
        //[HttpPost]
        //public async Task<IActionResult> Details(string itemNumber, int quantityDelivered, int quantityForVoucher, string remarkForDelivery, string remarkForVoucher, int quantityDeliveredModalName, int addToVoucherModalName, int confirmDeliveryModalName)
        //{
        //    ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
        //    List<string> identity = userCheck.checkUserIdentityAsync(user);
        //    int userID = user.WorkID;

        //    if (itemNumber == null)
        //    {
        //        return NotFound();
        //    }

        //    if (addToVoucherModalName == 1)
        //    {
        //        b.AddItemToVoucher(userID, itemNumber, quantityForVoucher, remarkForVoucher);
        //    }
        //    else if (quantityDeliveredModalName == 1)
        //    {
                
        //    }

        //    return View(result);
        //}

        private bool DisbursementListExists(string id)
        {
            return _context.DisbursementList.Any(e => e.Dlid == id);
        }
    }
}
