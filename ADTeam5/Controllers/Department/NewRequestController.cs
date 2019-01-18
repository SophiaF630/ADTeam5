using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;
using ADTeam5.BusinessLogic;


namespace ADTeam5.Controllers.Department
{
    public class NewRequestController : Controller
    {
        BizLogic b = new BizLogic();
            private readonly SSISTeam5Context context;
        static List<string> ItemNumberList= new List<string>();
        static List<int> QuantityList = new List<int>();

        public NewRequestController(SSISTeam5Context context)
            {
                this.context = context;
            }

            public IActionResult Index()
        {
            List<Catalogue> catalogueList = new List<Catalogue>();
            catalogueList = (from x in context.Catalogue select x).ToList();
            catalogueList.Insert(0, new Catalogue { ItemNumber = "0", ItemName = "Select" });
            ViewBag.ListofCatalogueName = catalogueList;
            
            return View();
        }

        [HttpPost]
        public IActionResult Index(string itemNumber, int quantity)
        {
            List<Catalogue> catalogueList = new List<Catalogue>();
            catalogueList = (from x in context.Catalogue select x).ToList();
            catalogueList.Insert(0, new Catalogue { ItemNumber = "0", ItemName = "Select" });
            ViewBag.ListofCatalogueName = catalogueList;

            string ItemNumber = itemNumber;
            ItemNumberList.Add(ItemNumber);
            int Quantity = quantity;
            QuantityList.Add(quantity);

            //TemporaryRecordDetails t = new TemporaryRecordDetails(ItemNumber, Quantity);

            ViewBag.ItemNumberList = ItemNumberList;
            ViewBag.QuantityList = QuantityList;

            return View();
        }

        // POST: Save orders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit()
        {
                // Make new EmployeeRequestRecord
                Models.EmployeeRequestRecord e = new Models.EmployeeRequestRecord();
                string id = b.IDGenerator("STAS");
                DateTime requestDate = DateTime.Now.Date;
                int empId = 11233;  //Filter according to userID
                int headId = 11213; //Filter according to dept head of person using this
                string deptCode = "STAS"; //Filter according to dept code of person using this
                string status = "Submitted";
                e.Rrid = id;
                e.RequestDate = requestDate;
                e.DepEmpId = empId;
                e.DepHeadId = headId;
                e.DepCode = deptCode;
                e.Status = status;
                context.EmployeeRequestRecord.Add(e);
                context.SaveChanges();

                //Make new Record Details
                for (int k = 0; k< QuantityList.Count; k++)
                {
                    Models.RecordDetails r = new Models.RecordDetails();
                    r.Rrid = id;
                    r.ItemNumber = ItemNumberList[k];
                    r.Quantity = QuantityList[k];
                context.RecordDetails.Add(r);
                    context.SaveChanges();
                }
            return Content("saved");
        }


    }
}