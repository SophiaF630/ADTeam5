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
        public string SupplierCode { get; set; }
        public string Status { get; set; }

        public virtual User StoreClerk { get; set; }
        public virtual Supplier SupplierCodeNavigation { get; set; }
    }
}
