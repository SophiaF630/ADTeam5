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

        public IActionResult Index()
        {
            SSISTeam5Context e = new SSISTeam5Context();
            ViewExpenditure ve = new ViewExpenditure();

            //ve.explist= (from x in e.Catalogue join b in e.RecordDetails on x.ItemNumber equals b.ItemNumber
            //             select new ViewExpenditure()
            //             {

            //             }
                
                
                //from x in e.Catalogue
                //        join b in e.RecordDetails
                //        on x.ItemNumber equals b.ItemNumber
                //        where x.ItemNumber== b.ItemNumber
                //        select new ViewExpenditure()
                //        {
                          
                         
                //        };




            return View();
        }

        public IActionResult Viewexpenditure()
        {
           


            return View();
        }
    }
}