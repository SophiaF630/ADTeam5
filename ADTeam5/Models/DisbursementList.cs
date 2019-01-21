using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class DisbursementList
    {
        public string Dlid { get; set; }
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Estimate Deliver Date")]
        public DateTime? EstDeliverDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:dd/MM/yyyy}")]
        [Display(Name = "Complete Date")]
        public DateTime? CompleteDate { get; set; }
        public string DepartmentCode { get; set; }
        public int RepId { get; set; }
        public int CollectionPointId { get; set; }
        public string Status { get; set; }

        public virtual CollectionPoint CollectionPointNavigation { get; set; }
        [Display(Name = "Department")]
        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual User RepNavigation { get; set; }
    }
}
