using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class TemporaryRecordDetails
    {
       
        string itemNumber;
        int quantity;
       // public List<TemporaryRecordDetails> tList{ get; set; }

        public TemporaryRecordDetails(string itemNumber, int quantity)
        {
            this.itemNumber = itemNumber;
            this.quantity = quantity;
            //this.tList = new List<TemporaryRecordDetails>();
        }

        //public int Id
        //{
        //    get { return id; }
        //    set { id = value; }
        //}
        public string ItemNumber
        {
            get { return itemNumber; }
            set { itemNumber = value; }
        }
        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
    }

}
