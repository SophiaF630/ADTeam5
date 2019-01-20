using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class DepartmentCoveringHeadRecord
    {
        public int Chindex { get; set; }

        public int UserId { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Start Date is mandatory")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "End Date is mandatory")]
        public DateTime EndDate { get; set; }

        public virtual User User { get; set; }
    }
}
