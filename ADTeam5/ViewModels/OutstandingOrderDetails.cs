using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class OutstandingOrderDetails
    {
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}
