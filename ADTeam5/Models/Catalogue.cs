using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class Catalogue
    {
        public Catalogue()
        {
            InventoryTransRecord = new HashSet<InventoryTransRecord>();
            RecordDetails = new HashSet<RecordDetails>();
        }

        [Display(Name = "Item No.")]
        [Required(ErrorMessage = "*This field is required")]
        //[ExcludeChar(@"!@#$%^&*().,<>?/:;'", ErrorMessage = "Invalid format")]
        [RegularExpression(@"([a-zA-Z0-9]+)", ErrorMessage = "*Invalid format, only alphabets or digit numbers are allowed")]
        //[RegularExpression(@"([a-zA-Z]{1})([0-9]{3})", ErrorMessage = "*Invalid format, please use a foramt of an alphabet followed by 3 digit numbers")]
        [MaxLength(4, ErrorMessage = "Maximum 4 digits")]
        public string ItemNumber { get; set; }
        [Required(ErrorMessage = "*This field is required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "*This field is required")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Reorder Level")]
        public int? ReorderLevel { get; set; }
        [Display(Name = "Reorder Quantity")]
        public int? ReorderQty { get; set; }
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }
        [ReadOnly(true)]
        public int Stock { get; set; }
        public string Supplier1 { get; set; }
        public string Supplier2 { get; set; }
        public string Supplier3 { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S{0:c}")]
        public decimal? Supplier1Price { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S{0:c}")]
        public decimal? Supplier2Price { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S{0:c}")]
        public decimal? Supplier3Price { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? Last1OrderDate { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? Last2OrderDate { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? Last3OrderDate { get; set; }
        public string Location { get; set; }

        [Display(Name = "Supplier 1")]
        public Supplier Supplier1Navigation { get; set; }
        [Display(Name = "Supplier 2")]
        public Supplier Supplier2Navigation { get; set; }
        [Display(Name = "Supplier 3")]
        public Supplier Supplier3Navigation { get; set; }

        public virtual ICollection<InventoryTransRecord> InventoryTransRecord { get; set; }
        public virtual ICollection<RecordDetails> RecordDetails { get; set; }
    }
}
