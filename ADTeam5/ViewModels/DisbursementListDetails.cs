using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class DisbursementListDetails
    {
        [Display(Name = "Item Number")]
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        [Display(Name = "Quantity Needed")]
        public int Quantity { get; set; }
        public string Remark { get; set; }

    }
}
