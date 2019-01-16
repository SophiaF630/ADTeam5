using System;
using System.Collections.Generic;

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

        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string GstregistrationNo { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string ContactName { get; set; }
        public int PhoneNo { get; set; }
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
