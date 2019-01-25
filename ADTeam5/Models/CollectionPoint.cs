using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class CollectionPoint
    {
        public CollectionPoint()
        {
            Department = new HashSet<Department>();
            DisbursementList = new HashSet<DisbursementList>();
        }
        [Required]
        public int CollectionPointId { get; set; }

        [Display(Name = "Collection Point")]
        //[StringLength(50)]
        public string CollectionPointName { get; set; }

        [Display(Name = "Collection Time")]
        public DateTime CollectionTime { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        public virtual ICollection<Department> Department { get; set; }
        public virtual ICollection<DisbursementList> DisbursementList { get; set; }
    }
}
