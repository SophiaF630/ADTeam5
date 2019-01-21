using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class EmployeeRequestRecord
    {
        public string Rrid { get; set; }

        [Display(Name ="Request Date")]
        [DataType(DataType.Date)]
        public DateTime RequestDate { get; set; }

        [DataType(DataType.Date)]
        [EndLaterThanV("RequestDate")]
        public DateTime? CompleteDate { get; set; }
        [Required]
        [Display(Name ="Employee ID")]
        public int DepEmpId { get; set; }
        public int? DepHeadId { get; set; }
        public string DepCode { get; set; }
        [StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        public string Status { get; set; }
        [StringLength(250, ErrorMessage ="Too long"), RegularExpression(@"^[a-zA-Z''-'\s]{1,200}$")]
        public string Remark { get; set; }

        public virtual Department DepCodeNavigation { get; set; }
        public virtual User DepEmp { get; set; }
        public virtual User DepHead { get; set; }
    }
}
