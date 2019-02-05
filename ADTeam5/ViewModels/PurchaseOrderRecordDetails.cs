using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class PurchaseOrderRecordDetails
    {
        [Display(Name = "No.")]
        public int RowID { get; set; }
        [Display(Name = "RDID")]
        public int RDID { get; set; }
        public string POID { get; set; }
        [Display(Name = "Item Number")]
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Quantity Delivered")]
        public int QuantityDelivered { get; set; }
        public decimal? Price { get; set; }
    }
}
