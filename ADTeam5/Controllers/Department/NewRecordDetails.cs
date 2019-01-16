using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.Controllers.Department
{
    public class NewRecordDetails
    {
        string itemnum;
        int qty;

        public NewRecordDetails()
        {

        }
        public NewRecordDetails (string itemnum, int qty)
        {
            this.itemnum = itemnum;
            this.qty = qty;
        }
        public string Itemnum
        {
            get { return Itemnum; }
            set { Itemnum = value; }
        }
        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }
    }
}
