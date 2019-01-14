using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ADTeam5.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            DateTime start = DateTime.Now;
            DateTime cutoff = DateTime.Now;
            if (start.DayOfWeek >= DayOfWeek.Thursday)
            {
                start = start.AddDays(-7);
            }
            while (start.DayOfWeek != DayOfWeek.Thursday)
            {
                start = start.AddDays(-1);
            }
            Console.WriteLine("{0}", start);

            if (cutoff.DayOfWeek >= DayOfWeek.Wednesday)
            {
                while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
                {
                    cutoff = cutoff.AddDays(-1);
                }
            }
            else
            {
                while (cutoff.DayOfWeek != DayOfWeek.Wednesday)
                {
                    cutoff = cutoff.AddDays(1);
                }
            }
            Console.WriteLine("{0}", cutoff);
            return View();
        }
    }
}