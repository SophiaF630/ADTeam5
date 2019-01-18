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
        public string RecordId { get; set; }
        [Required(ErrorMessage ="Please enter a quantity")]
        public int Qty { get; set; }
        public int Balance { get; set; }
        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
