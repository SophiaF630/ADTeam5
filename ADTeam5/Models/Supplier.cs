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

        [Display(Name = "Code")]
        public string SupplierCode { get; set; }
        [Display(Name = "Supplier")]
        [StringLength(50)]
        public string SupplierName { get; set; }
        [StringLength(12)]
        public string GstregistrationNo { get; set; }
        [StringLength(5)]
        public string TitleOfCourtesy { get; set; }
        [Display(Name = "Contact Name")]
        [StringLength(50)]
        public string ContactName { get; set; }
        [Display(Name = "Contact No.")]
        [MaxLength(10)]
        public int PhoneNo { get; set; }
        [Display(Name = "Fax No.")]
        [MaxLength(10)]
        public int? FaxNo { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [MaxLength(10)]
        public int? PostalCode { get; set; }

        public virtual ICollection<Catalogue> CatalogueSupplier1Navigation { get; set; }
        public virtual ICollection<Catalogue> CatalogueSupplier2Navigation { get; set; }
        public virtual ICollection<Catalogue> CatalogueSupplier3Navigation { get; set; }
        public virtual ICollection<PurchaseOrderRecord> PurchaseOrderRecord { get; set; }
    }
}
