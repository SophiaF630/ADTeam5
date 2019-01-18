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
        //BizLogic b = new BizLogic();
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
        public IActionResult Submit()
        {
            
            for(int i = 0; i < QuantityList.Count; i++)
            {
                Models.EmployeeRequestRecord e = new Models.EmployeeRequestRecord();
                
                Models.RecordDetails r = new Models.RecordDetails();
                //QuantityList[0] = 
            }
           

            return View();
        }


    }
}