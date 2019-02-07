using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class StationeryUsageViewModel
    {
        [Display(Name = "No.")]
        public int RowID { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Complete Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? CompleteDate { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        [Display(Name = "Quantity Delivered")]
        public int QuantityDelivered { get; set; }
        [Display(Name = "Department")]
        public string DepCode { get; set; }
    }
}
