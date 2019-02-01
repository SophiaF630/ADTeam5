using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class ChargeBackViewModel
    {
        [Display(Name = "No.")]
        public int RowID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Display(Name = "Department")]
        public string DepCode { get; set; }
        [Display(Name = "TotalAmount")]
        public decimal? TotalAmount { get; set; }
    }
}
