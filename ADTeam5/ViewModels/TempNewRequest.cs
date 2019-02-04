using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class TempNewRequest
    {
        [Display(Name = "No.")]
        public int RowID { get; set; }
        [Display(Name = "Item Number")]
        public string ItemNumber { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
    }
}
