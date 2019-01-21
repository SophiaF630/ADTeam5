using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class PurchaseOrderRecord
    {
        public string Poid { get; set; }
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ExpectedCompleteDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? CompleteDate { get; set; }
        public int StoreClerkId { get; set; }
        [StringLength(4)]
        public string SupplierCode { get; set; }
        [StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$", ErrorMessage ="Letters only")]
        public string Status { get; set; }

        public virtual User StoreClerk { get; set; }
        public virtual Supplier SupplierCodeNavigation { get; set; }
    }
}
