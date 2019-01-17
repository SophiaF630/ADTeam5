using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class InventoryTransRecord
    {
        public int TransId { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string ItemNumber { get; set; }
        public string RecordId { get; set; }
        public int Qty { get; set; }
        public int Balance { get; set; }

        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
