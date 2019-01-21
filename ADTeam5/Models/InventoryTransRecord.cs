using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class InventoryTransRecord
    {
        public int TransId { get; set; }
        [DataType(DataType.Date)]
        [NotBeforeTodayV]
        public DateTime Date { get; set; }
        [MaxLength(5)]
        public string ItemNumber { get; set; }
        [StringLength(50)]
        public string RecordId { get; set; }
        [Required(ErrorMessage ="Please enter a quantity")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        public int Qty { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        [Range(0, 10000, ErrorMessage = "Amount cannot be negative")]
        public int Balance { get; set; }
        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
