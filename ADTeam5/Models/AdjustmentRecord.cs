using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class AdjustmentRecord
    {

        public string VoucherNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime IssueDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ApproveDate { get; set; }
        public int ClerkId { get; set; }
        public int? SuperviserId { get; set; }
        public int? ManagerId { get; set; }
        public string Status { get; set; }

        public virtual User Clerk { get; set; }
        public virtual User Manager { get; set; }
        public virtual User Superviser { get; set; }
    }
}
