using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.Models
{
    public class Item
    {
        [Required, RegularExpression(@"^[A-Z]+[a-zA-Z'\s]*$")]
        public string ItemName { get; set; }
        [Range(0, 10000, ErrorMessage ="Please enter a valid quantity")]
        public int Quantity { get; set; }

    }
}
