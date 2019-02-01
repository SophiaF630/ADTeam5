using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace ADTeam5.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            CatalogueSupplier1Navigation = new HashSet<Catalogue>();
            CatalogueSupplier2Navigation = new HashSet<Catalogue>();
            CatalogueSupplier3Navigation = new HashSet<Catalogue>();
            PurchaseOrderRecord = new HashSet<PurchaseOrderRecord>();
        }
        [Required]
        [Display(Name = "Code")]
        public string SupplierCode { get; set; }
        [Required]
        [Display(Name = "Supplier")]
        [StringLength(50)]
        public string SupplierName { get; set; }
        [Required]
        [StringLength(12)]
        public string GstregistrationNo { get; set; }
        [Required]
        [StringLength(5)]
        public string TitleOfCourtesy { get; set; }
        [Required]
        [Display(Name = "Contact Name")]
        [StringLength(50)]
        public string ContactName { get; set; }
        [Required]
        [Display(Name = "Contact No.")]
        public int PhoneNo { get; set; }
        [Required]
        [Display(Name = "Fax No.")]
        public int? FaxNo { get; set; }
        [Required]
        [StringLength(250)]
        public string Address { get; set; }
        [Required]
        [StringLength(50)]
        public string City { get; set; }
        [Required]
        public int? PostalCode { get; set; }

        public virtual ICollection<Catalogue> CatalogueSupplier1Navigation { get; set; }
        public virtual ICollection<Catalogue> CatalogueSupplier2Navigation { get; set; }
        public virtual ICollection<Catalogue> CatalogueSupplier3Navigation { get; set; }
        public virtual ICollection<PurchaseOrderRecord> PurchaseOrderRecord { get; set; }
    }
}
