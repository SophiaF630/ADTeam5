using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Models;
using ADTeam5.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
namespace ADTeam5.Controllers.Department
{
    public class AddRequestController : Controller
    {

        private readonly SSISTeam5Context context;

        public AddRequestController(SSISTeam5Context context)
        {
            this.context = context;
        }

        public ViewResult Index()
        {
            SSISTeam5Context e = new SSISTeam5Context();

            List<ViewExpenditure> ve = new List<ViewExpenditure>();
            
           
            var q = (from x in e.Catalogue
                     join b in e.RecordDetails on x.ItemNumber equals b.ItemNumber
                     join c in e.DisbursementList on b.Rrid equals c.Dlid
                     where c.Status.Contains("completed")
                     
                     select new ViewExpenditure()
                     {
                         quantity = b.Quantity,
                         price = x.Supplier1Price,
                         subtotal = b.Quantity * x.Supplier1Price,
                         orderno = c.Dlid,
                         status = c.Status,
                         CompleteDate= c.CompleteDate

                     }).ToList();
            DateTime test = DateTime.Parse("2019/01/10");

            ViewExpenditure a = new ViewExpenditure();

        


            return View(q);
        }

        public IActionResult Viewexpenditure()
        {
           


            return View();
        }
    }
}