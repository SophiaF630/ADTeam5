using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class DisbursementList
    {
        public string Dlid { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public string DepartmentCode { get; set; }
        public int RepId { get; set; }
        public int CollectionPointId { get; set; }
        public string Status { get; set; }

        public virtual CollectionPoint CollectionPoint { get; set; }
        public virtual Department DepartmentCodeNavigation { get; set; }
        public virtual User Rep { get; set; }
    }
}
