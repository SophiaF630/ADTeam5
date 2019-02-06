using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.BusinessLogic;
using ADTeam5.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Routing;

namespace ADTeam5.Controllers
{
    public class NewRequestController : Controller
    {
        static int userid;
        static string dept;
        static string role;

        private readonly SSISTeam5Context _context;
        private readonly UserManager<ADTeam5User> _userManager;
        readonly GeneralLogic userCheck;

        BizLogic b = new BizLogic();
        static List<string> ItemNumberList = new List<string>();
        static List<string> ItemNameList = new List<string>();
        static List<int> QuantityList = new List<int>();
        static List<TempNewRequest> tempNewRequests = new List<TempNewRequest>();
        string rrid;

        public NewRequestController(SSISTeam5Context context, UserManager<ADTeam5User> userManager)
        {
            _context = context;
            _userManager = userManager;
            userCheck = new GeneralLogic(_context);
        }

        public async Task<IActionResult> Index()
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
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

            
            return View(tempNewRequests);
            
        }

        //POST: Save orders
        [HttpPost]
        public async Task<IActionResult> Index(string[] itemSubmitted)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            int userID = user.WorkID;
            dept = identity[0];
            role = identity[1];

            if (itemSubmitted.Length != 0)
            {
                rrid = b.IDGenerator(dept);
                foreach (var item in tempNewRequests)
                {
                    if (Array.Exists(itemSubmitted, i => i == item.ItemNumber.ToString()))
                    {
                        //add items to request
                        RecordDetails r = new RecordDetails();
                        r.Rrid = rrid;
                        r.ItemNumber = item.ItemNumber;
                        r.Quantity = item.Quantity;
                        r.QuantityDelivered = 0;
                        _context.RecordDetails.Add(r);
                        _context.SaveChanges();                       
                    }
                }

                //check if item exists in record details
                var record = _context.RecordDetails.FirstOrDefault(x => x.Rrid == rrid);
                if (record != null)
                {
                    // Make new EmployeeRequestRecord
                    EmployeeRequestRecord e = new EmployeeRequestRecord();
                    DateTime requestDate = DateTime.Now.Date;
                    int empId = userid;
                    var findHeadId = _context.Department.Where(x => x.DepartmentCode == dept).First();
                    int headId = findHeadId.HeadId;
                    string deptCode = dept;
                    string status = "Pending Approval";
                    e.Rrid = rrid;
                    e.RequestDate = requestDate;
                    e.DepEmpId = empId;
                    e.DepHeadId = headId;
                    e.DepCode = deptCode;
                    e.Status = status;
                    _context.EmployeeRequestRecord.Add(e);
                    _context.SaveChanges();

                    TempData["Submitted"] = "New request has been created successfully!";
                }
            }

            tempNewRequests = new List<TempNewRequest>();
            List<RecordDetails> rdList = _context.RecordDetails.Where(x => x.Rrid == rrid).ToList();

            //return RedirectToAction("Details", new { id = rrid });          
            return Json(new { redirecturl = "NewRequest/Details/"+ rrid });
        }


        [HttpPost]
        public IActionResult AddItem(string itemNumber, int quantity)
        {
            //check if item exists
            int rowID = 1;
            var request = tempNewRequests.FirstOrDefault(x => x.ItemNumber == itemNumber);
            if (request != null)
            {
                request.Quantity += quantity;
            }
            else
            {
                TempNewRequest tempNewRequest = new TempNewRequest();
                tempNewRequest.RowID = rowID;
                tempNewRequest.ItemNumber = itemNumber;
                tempNewRequest.ItemName = _context.Catalogue.Find(itemNumber).ItemName;
                tempNewRequest.Quantity = quantity;
                tempNewRequests.Add(tempNewRequest);

                rowID++;
            }

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            ViewData["SubmitButton"] = "true";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditItem(string itemName, int quantity)
        {
            var request = tempNewRequests.FirstOrDefault(x => x.ItemName == itemName);
            if (request != null)
            {
                request.Quantity = quantity;
            }
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            ViewData["RRID"] = id;
            var q = _context.RecordDetails.Where(x => x.Rrid == id);

            return View(q);
        }




       
        [HttpPost]
        public async Task<IActionResult> RequestItemDelete(string itemNumber)
        {
            ADTeam5User user = await _userManager.GetUserAsync(HttpContext.User);
            userid = user.WorkID;
            List<string> identity = userCheck.checkUserIdentityAsync(user);
            dept = identity[0];
            role = identity[1];

            //Viewbag for category dropdown list, need to post back
            List<Catalogue> categoryList = new List<Catalogue>();
            var q = _context.Catalogue.GroupBy(x => new { x.Category }).Select(x => x.FirstOrDefault());
            foreach (var item in q)
            {
                categoryList.Add(item);
            }
            categoryList.Insert(0, new Catalogue { ItemNumber = "0", Category = "---Select Category---" });
            ViewBag.ListofCategory = categoryList;

            var deleteItem = tempNewRequests.FirstOrDefault(x => x.ItemNumber == itemNumber);
            tempNewRequests.Remove(deleteItem);

            if (tempNewRequests == null)
            {
                tempNewRequests = new List<TempNewRequest>();
                
            }
            return PartialView("_TempNewRequest", tempNewRequests);

        }


        public JsonResult GetItemNameList(string category)
        {

            List<Catalogue> itemNameList = _context.Catalogue.Where(x => x.Category == category).ToList();
            return Json(itemNameList);
        }
    }
}