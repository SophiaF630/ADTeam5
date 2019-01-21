using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.Models
{
    public class Item
    {
        [MaxLength(50)]
        [Required(ErrorMessage = "*This field is required")]
        public string ItemName { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        [Range(0, 10000, ErrorMessage ="Please enter a valid quantity")]
        public int Quantity { get; set; }

    }
}
