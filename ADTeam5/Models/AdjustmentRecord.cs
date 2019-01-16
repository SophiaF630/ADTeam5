using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class AdjustmentRecord
    {
        public string VoucherNo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ApproveDate { get; set; }
        public int ClerkId { get; set; }
        public int SuperviserId { get; set; }
        public int? ManagerId { get; set; }
        public string Status { get; set; }

        public virtual User Clerk { get; set; }
        public virtual User Manager { get; set; }
        public virtual User Superviser { get; set; }
    }
}
