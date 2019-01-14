using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class User
    {
        public User()
        {
            AdjustmentRecord = new HashSet<AdjustmentRecord>();
            DepartmentCoveringHeadRecord = new HashSet<DepartmentCoveringHeadRecord>();
            EmployeeRequestRecord = new HashSet<EmployeeRequestRecord>();
            PurchaseOrderRecord = new HashSet<PurchaseOrderRecord>();
        }

        public int UserId { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string Name { get; set; }
        public string DepartmentCode { get; set; }
        public int? Telephone { get; set; }
        public int? Fax { get; set; }
        public string EmailAddress { get; set; }

        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual ICollection<AdjustmentRecord> AdjustmentRecord { get; set; }
        public virtual ICollection<DepartmentCoveringHeadRecord> DepartmentCoveringHeadRecord { get; set; }
        public virtual ICollection<EmployeeRequestRecord> EmployeeRequestRecord { get; set; }
        public virtual ICollection<PurchaseOrderRecord> PurchaseOrderRecord { get; set; }
    }
}
