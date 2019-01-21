using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class Department
    {
        public Department()
        {
            DisbursementList = new HashSet<DisbursementList>();
            EmployeeRequestRecord = new HashSet<EmployeeRequestRecord>();
            User = new HashSet<User>();
        }

        [StringLength(5)]
        public string DepartmentCode { get; set; }
        [StringLength(50)]
        [Display(Name="Department Name")]
        public string DepartmentName { get; set; }
        public int CollectionPointId { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        public int HeadId { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        public int RepId { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        public int? CoveringHeadId { get; set; }
        [StringLength(50)]
        public string CollectionPassword { get; set; }

        public virtual CollectionPoint CollectionPoint { get; set; }
        public virtual User CoveringHead { get; set; }
        public virtual User Head { get; set; }
        public virtual User Rep { get; set; }
        public virtual ICollection<DisbursementList> DisbursementList { get; set; }
        public virtual ICollection<EmployeeRequestRecord> EmployeeRequestRecord { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
