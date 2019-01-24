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
        [Display(Name="Name")]
        public string SupplierName { get; set; }        
        public string GstregistrationNo { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string ContactName { get; set; }
        [Display(Name = "Phone")]
        public int PhoneNo { get; set; }
        [Display(Name = "Fax")]
        public int? FaxNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int? PostalCode { get; set; }

        public virtual ICollection<Catalogue> CatalogueSupplier1Navigation { get; set; }
        public virtual ICollection<Catalogue> CatalogueSupplier2Navigation { get; set; }
        public virtual ICollection<Catalogue> CatalogueSupplier3Navigation { get; set; }
        public virtual ICollection<PurchaseOrderRecord> PurchaseOrderRecord { get; set; }
    }
}
