
using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class User
    {
        public User()
        {
            AdjustmentRecordClerk = new HashSet<AdjustmentRecord>();
            AdjustmentRecordManager = new HashSet<AdjustmentRecord>();
            AdjustmentRecordSuperviser = new HashSet<AdjustmentRecord>();
            DepartmentCoveringHead = new HashSet<Department>();
            DepartmentCoveringHeadRecord = new HashSet<DepartmentCoveringHeadRecord>();
            DepartmentHead = new HashSet<Department>();
            DepartmentRep = new HashSet<Department>();
            DisbursementList = new HashSet<DisbursementList>();
            EmployeeRequestRecordDepEmp = new HashSet<EmployeeRequestRecord>();
            EmployeeRequestRecordDepHead = new HashSet<EmployeeRequestRecord>();
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
        public virtual ICollection<AdjustmentRecord> AdjustmentRecordClerk { get; set; }
        public virtual ICollection<AdjustmentRecord> AdjustmentRecordManager { get; set; }
        public virtual ICollection<AdjustmentRecord> AdjustmentRecordSuperviser { get; set; }
        public virtual ICollection<Department> DepartmentCoveringHead { get; set; }
        public virtual ICollection<DepartmentCoveringHeadRecord> DepartmentCoveringHeadRecord { get; set; }
        public virtual ICollection<Department> DepartmentHead { get; set; }
        public virtual ICollection<Department> DepartmentRep { get; set; }
        public virtual ICollection<DisbursementList> DisbursementList { get; set; }
        public virtual ICollection<EmployeeRequestRecord> EmployeeRequestRecordDepEmp { get; set; }
        public virtual ICollection<EmployeeRequestRecord> EmployeeRequestRecordDepHead { get; set; }
        public virtual ICollection<PurchaseOrderRecord> PurchaseOrderRecord { get; set; }

    }
}
