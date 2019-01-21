﻿using System;
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


        [Required]
        [Display(Name = "Item Number")]
        public string ItemNumber { get; set; }
        [Required]
        [Display(Name ="Item Name")]
        [StringLength(50)]
        public string ItemName { get; set; }
        [Required]
        [Display(Name = "Quantity Needed")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        [Range(0, 10000, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }


    }
}
