using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class AdjustmentRecordViewModel
    {
        [Display(Name = "Voucher No.")]
        [StringLength(25)]
        public string VoucherNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime IssueDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]

        public DateTime? ApproveDate { get; set; }
        public int ClerkId { get; set; }
        public string ClerkName { get; set; }
        public int? SuperviserId { get; set; }
        public string SupervisorName { get; set; }
        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }
        [StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$", ErrorMessage = "Letters only")]
        public string Status { get; set; }

    }
}
