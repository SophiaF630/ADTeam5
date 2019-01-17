using ADTeam5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class VMCatalogue
    {
        public Catalogue Catalogue{ get; set; }
        public IEnumerable<Supplier> Suppliers { get; set; }
    }
}
