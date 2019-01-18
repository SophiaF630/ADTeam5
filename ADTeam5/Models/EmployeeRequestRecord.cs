using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class EmployeeRequestRecord
    {
        public string Rrid { get; set; }
        [DataType(DataType.Date)]
        [NotBeforeTodayV]
        public DateTime RequestDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CompleteDate { get; set; }
        [Display(Name ="Employee ID")]
        public int DepEmpId { get; set; }
        public int? DepHeadId { get; set; }
        [MaxLength(4, ErrorMessage = "Invalid Department Code")]
        public string DepCode { get; set; }
        [StringLength(10), RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        public string Status { get; set; }
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,200}$")]
        public string Remark { get; set; }
        public virtual Department DepCodeNavigation { get; set; }
        public virtual User DepEmp { get; set; }
        public virtual User DepHead { get; set; }
    }
}
