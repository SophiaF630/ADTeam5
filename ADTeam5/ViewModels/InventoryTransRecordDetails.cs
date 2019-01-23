using ADTeam5.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class InventoryTransRecordDetails
    {
        public int TransId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }
        public string ItemName { get; set; }
        public string DepartmentOrSupplier { get; set; }
        public int Qty { get; set; }
        public int Balance { get; set; }

        public virtual Catalogue ItemNumberNavigation { get; set; }
    }
}
