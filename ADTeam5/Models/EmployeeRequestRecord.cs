using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class EmployeeRequestRecord
    {
        public string Rrid { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public int DepEmpId { get; set; }
        public int? DepHeadId { get; set; }
        public int? DepRepId { get; set; }
        public string DepCode { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }

        public virtual User DepEmp { get; set; }
    }
}
