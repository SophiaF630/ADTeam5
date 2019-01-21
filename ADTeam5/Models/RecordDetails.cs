using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class RecordDetails
    {
        [Required]
        public int Rdid { get; set; }
        [StringLength(50)]
        public string Rrid { get; set; }
        [StringLength(5)]
        public string ItemNumber { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        [Range(0, 10000, ErrorMessage = "Amount cannot be negative")]
        public int Quantity { get; set; }
        [StringLength(250, ErrorMessage ="Too long"), RegularExpression(@"^[a-zA-Z''-'\s]{1,200}$")]
        public string Remark { get; set; }

        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
