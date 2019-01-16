using System;
using System.Collections.Generic;

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

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int CollectionPointId { get; set; }
        public int HeadId { get; set; }
        public int RepId { get; set; }
        public int? CoveringHeadId { get; set; }
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
