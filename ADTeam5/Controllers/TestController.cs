using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.BusinessLogic;
using ADTeam5.Models;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class TestController : Controller
    {
        //public IActionResult Index()
        //{
        //    DateTime start = DateTime.Now;
        //    DateTime cutoff = DateTime.Now;
        //    if (start.DayOfWeek >= DayOfWeek.Thursday)
        //    {
        //        start = start.AddDays(-7);
        //    }
        //    while (start.DayOfWeek != DayOfWeek.Thursday)
        //    {
        //        start = start.AddDays(-1);
        //    }
        //    Console.WriteLine("{0}", start);

        //    if (cutoff.DayOfWeek >= DayOfWeek.Wednesday)
        //    {
        //        while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
        //        {
        //            cutoff = cutoff.AddDays(-1);
        //        }
        //    }
        //    else
        //    {
        //        while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
        //        {
        //            cutoff = cutoff.AddDays(1);
        //        }
        //    }
        //    Console.WriteLine("{0}", cutoff);
        //    return View();
        //}

        public IActionResult Index()
        {
            BizLogic b = new BizLogic();
            string test = new BizLogic().IDGenerator("DL");
            Console.WriteLine("{0}", test);

            List<RecordDetails> rd = b.GenerateDisbursementListDetails("ENGL");
            foreach(RecordDetails i in rd)
            {
                Console.WriteLine("{0}, {1}, {2}", i.Rrid, i.ItemNumber, i.Quantity);
            }
            return View();
        }
    }
}