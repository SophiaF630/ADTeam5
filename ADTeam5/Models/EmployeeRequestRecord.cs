using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class EmployeeRequestRecord
    {
        public string Rrid { get; set; }

        [DataType(DataType.Date)]
        
        public DateTime RequestDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompleteDate { get; set; }

        public int DepEmpId { get; set; }
        public int? DepHeadId { get; set; }
        public string DepCode { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }

        public virtual Department DepCodeNavigation { get; set; }
        public virtual User DepEmp { get; set; }
        public virtual User DepHead { get; set; }
    }
}
