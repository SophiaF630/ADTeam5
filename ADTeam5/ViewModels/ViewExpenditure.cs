using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class ViewExpenditure
    {
        public string status { get; set; }
        public decimal? subtotal { get; set; }
        public decimal? price { get; set; }
        public int quantity { get; set; }
        public IList<ViewExpenditure> explist { get; set; }
        public string orderno { get; set; }
        public string itemname { get; set; }

    }

    public class Renderview
    {
        [Display(Name = "Item Category")]
        public string Category { get; set; }
        //public IEnumerable< ViewExpenditure> ViewExpenditures { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Subtotal")]
        public decimal? Subtotal { get; set; }
        
    }
}
