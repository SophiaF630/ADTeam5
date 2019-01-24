using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class RecordDetails
    {
        public int Rdid { get; set; }
        public string Rrid { get; set; }
        public string ItemNumber { get; set; }
        public int Quantity { get; set; }
        public int QuantityDelivered { get; set; }
        public string Remark { get; set; }

        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
