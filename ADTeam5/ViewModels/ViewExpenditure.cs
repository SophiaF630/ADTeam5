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
        public string orderno { get; set; }
        public string status { get; set; }
        public decimal? subtotal { get; set; }
        public decimal? price { get; set; }
        public int quantity { get; set; }
        [Display(Name = "Complete Date")]
        public DateTime? CompleteDate { get; set; }
        public string itemname { get; set; }


    }

}
