using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class StationeryRetrievalList
    {
        //private object x;

        //public StationeryRetrievalList(string itemNumber, string itemName, int quantity)
        //{
        //    ItemNumber = itemNumber;
        //    ItemName = itemName;
        //    Quantity = quantity;
        //}

        //public StationeryRetrievalList(object x)
        //{
        //    this.x = x;
        //}

        [Display(Name = "Item Number")]
        public string ItemNumber { get; set; }
        public string ItemName { get; set; }
        [Display(Name = "Quantity Needed")]
        public int Quantity { get; set; }


    }
}
