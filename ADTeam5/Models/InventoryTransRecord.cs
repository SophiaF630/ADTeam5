using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class InventoryTransRecord
    {
        public int TransId { get; set; }
        public DateTime Date { get; set; }
        public string ItemNumber { get; set; }
        public string DepOrSupplier { get; set; }
        public string Qty { get; set; }
        public int Balance { get; set; }

        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
