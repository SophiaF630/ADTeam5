using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class DisbursementList
    {
        public string Dlid { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Estimate Deliver Date")]
        public DateTime? EstDeliverDate { get; set; }
        [Display(Name = "Complete Date")]
        public DateTime? CompleteDate { get; set; } 
        public string DepartmentCode { get; set; }
        public int RepId { get; set; }
        public int CollectionPointId { get; set; }
        public string Status { get; set; }

        public virtual CollectionPoint CollectionPointNavigation { get; set; }
        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual User RepNavigation { get; set; }

    }
}
