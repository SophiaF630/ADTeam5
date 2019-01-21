using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class PurchaseOrderRecord
    {
        [Required(ErrorMessage ="This field is required")]
        public string Poid { get; set; }
        [Display(Name ="Order Date")]
        [Required(ErrorMessage ="This field is required"), DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        [Required, DataType(DataType.Date)]
        [EndLaterThanV("OrderDate")]
        public DateTime? ExpectedCompleteDate { get; set; }
        [Required, DataType(DataType.Date)]
        [EndLaterThanV("OrderDate")]
        public DateTime? CompleteDate { get; set; }
        [ReadOnly(true)]
        public int StoreClerkId { get; set; }
        [StringLength(4)]
        public string SupplierCode { get; set; }
        [StringLength(50), RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$", ErrorMessage ="Letters only")]
        public string Status { get; set; }
        public virtual User StoreClerk { get; set; }
        public virtual Supplier SupplierCodeNavigation { get; set; }
    }
}
