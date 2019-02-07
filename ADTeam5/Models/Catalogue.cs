using ADTeam5.Validations;
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
        [RegularExpression(@"([a-zA-Z0-9]+)", ErrorMessage = "*Invalid format, only alphabets or numbers are allowed")]
        //[RegularExpression(@"([a-zA-Z]{1})([0-9]{3})", ErrorMessage = "*Invalid format, please use a foramt of an alphabet followed by 3 digit numbers")]
        [MaxLength(4, ErrorMessage = "Maximum 4 digits")]
        public string ItemNumber { get; set; }
        [Required(ErrorMessage = "*This field is required")]
        public string Category { get; set; }
        [Required(ErrorMessage = "*This field is required")]
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Reorder Level")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage ="Numbers only")]
        [Range(0,10000, ErrorMessage ="Reorder Level cannot be negative")]
        public int? ReorderLevel { get; set; }
        [Display(Name = "Reorder Quantity")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        [Range(0, 10000, ErrorMessage = "Reorder Quantity cannot be negative")]
        public int ReorderQty { get; set; }
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }
        [ReadOnly(true)]
        public int Stock { get; set; }
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Numbers only")]
        [Range(0, 10000, ErrorMessage = "Amount cannot be negative")]
        public int Out { get; set; }
        [StringLength(4)]
        [Required(ErrorMessage = "*This field is required")]
        //[ExcludeChar(@"!@#$%^&*().,<>?/:;'-", ErrorMessage = "Please select a supplier")]
        //[NotNull("")]
        [Display(Name = "Supplier 1")]
        public string Supplier1 { get; set; }
        [StringLength(4)]
        //[ExcludeChar(@"!@#$%^&*().,<>?/:;'-", ErrorMessage = "Please select a supplier")]
        //[NotNull("")]
        [Required(ErrorMessage = "*This field is required")]
        [Display(Name = "Supplier 2")]
        public string Supplier2 { get; set; }
        //[ExcludeChar(@"!@#$%^&*().,<>?/:;'-", ErrorMessage = "Please select a supplier")]
        //[NotNull("")]
        [StringLength(4)]
        [Display(Name = "Supplier 3")]
        public string Supplier3 { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S${0:0.00}")]
        [Range(0, 10000, ErrorMessage = "Price cannot be negative")]
        [Display(Name = "Supplier 1 Price")]
        public decimal? Supplier1Price { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S${0:0.00}")]
        [Range(0, 10000, ErrorMessage = "Price cannot be negative")]
        public decimal? Supplier2Price { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "S${0:0.00}")]
        [Range(0, 10000, ErrorMessage = "Price cannot be negative")]
        public decimal? Supplier3Price { get; set; }
        [ScaffoldColumn(false)]
        [DataType(DataType.Date)]
        public DateTime? Last1OrderDate { get; set; }
        [ScaffoldColumn(false)]
        [DataType(DataType.Date)]
        public DateTime? Last2OrderDate { get; set; }
        [ScaffoldColumn(false)]
        [DataType(DataType.Date)]
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
