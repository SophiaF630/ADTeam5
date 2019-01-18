using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class ViewExpenditure
    {
        public string status { get; set; }
        public decimal? subtotal { get; set; }
        public decimal? price { get; set; }
        public int quantity { get; set; }
        public IList<ViewExpenditure> explist { get; set; }

    }

}
