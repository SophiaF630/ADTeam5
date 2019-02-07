using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class TempVoucherDetails
    {

        [Display(Name = "No.")]
        public int RowID { get; set; }
        [Display(Name = "RDID")]
        public int RDID { get; set; }
        [Display(Name = "Item Number")]
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S${0:0.00}")]
        public decimal? Price { get; set; }
        public string Remark { get; set; }

        public bool IsChecked { get; set; }
    }
}
