using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ADTeam5.Controllers.Department
{
    public class NewRequestController : Controller
    {
        public static List<NewRecordDetails> list = new List<NewRecordDetails>();

        //For new record details
        string newItemnum;
        int newQty;

        //For new request
        static string newRrid;
        static DateTime newRequestDate;
        static int newDepHeadId;
        static int newDepEmpId;
        static string newDepCode;
        static string newStatus;

        private readonly SSISTeam5Context context;

        public NewRequestController(SSISTeam5Context context)
        {
            this.context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            ViewData["ItemName"] = new SelectList(context.Catalogue, "ItemNumber", "ItemName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem([Bind("ItemNumber, Quantity")]NewRecordDetails d)
        {
            if(ModelState.IsValid)
            {
                list.Add(d);   
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

