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
using Microsoft.AspNetCore.Identity.UI.Services;


namespace ADTeam5.Controllers
{
    public class IssueVoucherController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ADTeam5User> _userManager;
        private readonly SSISTeam5Context _context;
        BizLogic b = new BizLogic();
        readonly GeneralLogic userCheck;

        static List<string> ItemNumberList = new List<string>();
        static List<int> QuantityList = new List<int>();
        static string voucherNo = "";

        public IssueVoucherController(SSISTeam5Context context, UserManager<ADTeam5User> userManager, IEmailSender emailSender)
        {
           _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(context);
            _emailSender = emailSender;
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
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            List<Catalogue> itemNameList = new List<Catalogue>();
            itemNameList = (from x in _context.Catalogue select x).ToList();
            itemNameList.Insert(0, new Catalogue { ItemNumber = "0", ItemName = "---Select Item---" });
            ViewBag.ListofItemName = itemNameList;

            return View(tempVoucherDetailsList);
        }

        // POST: IssueVoucher
        [HttpPost]
        public async Task<IActionResult> Index(string itemNumber, int quantity, int rowID, string remark, int createNewVoucherItemModalName, int voucherItemModalName, string[] itemSubmitted, string[] itemSavedToDraft)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            voucherNo = "";            

            //handle post action
            List<TempVoucherDetails> tempVoucherDetailsList = b.GetTempVoucherDetailsList(userID);

            if (createNewVoucherItemModalName == 1)
            {
                b.CreateNewVoucherItem(userID, itemNumber, quantity, remark);
                return RedirectToAction(nameof(Index));
            }
            else if (voucherItemModalName == 1)
            {
                b.UpdateVoucherItem(rowID, quantity, remark, tempVoucherDetailsList);
            }

            if (itemSubmitted.Length != 0)
            {
                voucherNo = b.IDGenerator("V");
                foreach (var item in tempVoucherDetailsList)
                {
                    if (Array.Exists(itemSubmitted, i => i == item.RowID.ToString()))
                    {
                        b.AddItemsToVoucher(item.RowID, voucherNo, tempVoucherDetailsList);
                        b.CreateAdjustmentRecord(userID, voucherNo, "Submitted");

                        //send success email to staff
                        var staff = _context.User.Where(x => x.UserId == userID).First();
                        string email = staff.EmailAddress;
                        await _emailSender.SendEmailAsync(email, "Department Representative Appointment", "Dear " + staff.Name + ",<br>You have been appointed as the department representative for stationery collection.");

                        //send notification email to supervisor

                        var bosstest = (from User in _context.User
                                   join Department in _context.Department
                                   on User.UserId equals Department.HeadId
                                   where Department.DepartmentCode == "STAS"
                                   select new User {EmailAddress = User.EmailAddress, Name = User.Name }).ToList();

                        //var boss = from User in _context.User
                        //          join Department in _context.Department
                        //            on User.UserId equals Department.HeadId
                                    
                        //          select new { User.EmailAddress, User.Name };

                        ////string email2 = boss.EmailAddress;
                        //await _emailSender.SendEmailAsync(email2, "Department Representative Appointment", "Dear " + boss.Name + ",<br>You have been appointed as the department representative for stationery collection.");
                    }
                }
                //return RedirectToAction(nameof(Index));
            }
            else if(itemSavedToDraft.Length != 0)
            {
                voucherNo = b.IDGenerator("V");
                foreach (var item in tempVoucherDetailsList)
                {
                    if (Array.Exists(itemSavedToDraft, i => i == item.RowID.ToString()))
                    {
                        b.AddItemsToVoucher(item.RowID, voucherNo, tempVoucherDetailsList);
                        b.CreateAdjustmentRecord(userID, voucherNo, "Draft");
                    }
                }
                //return RedirectToAction(nameof(Index));
            }

            List<TempVoucherDetails> result = b.GetTempVoucherDetailsList(userID);

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;
            return View(result);
        }
       

        [HttpPost]
        //[ActionName("VoucherItemDelete"), Route("~/IssueVoucher")]
        public async Task<IActionResult> VoucherItemDelete(int id)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            List<TempVoucherDetails> tempVoucherDetailsList1 = b.GetTempVoucherDetailsList(userID);

            b.DeleteVoucherItem(id, tempVoucherDetailsList1);

            List<TempVoucherDetails> tempVoucherDetailsList = b.GetTempVoucherDetailsList(userID);

            if(tempVoucherDetailsList == null)
            {
                tempVoucherDetailsList = new List<TempVoucherDetails>();
            }
            return PartialView("_TempVoucherDetailsList", tempVoucherDetailsList);

        }

        

        private bool RecordDetailsExists(int id)
        {
            return _context.RecordDetails.Any(e => e.Rdid == id);
        }

        public JsonResult GetItemNameList(string category)
        {
            
            List<Catalogue> itemNameList = _context.Catalogue.Where(x => x.Category == category).ToList();
            return Json(itemNameList);
        }
    }
}
