using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class CollectionPoint
    {
        public CollectionPoint()
        {
            Department = new HashSet<Department>();
            DisbursementList = new HashSet<DisbursementList>();
        }

        public int CollectionPointId { get; set; }
        public string CollectionPointName { get; set; }
        public DateTime CollectionTime { get; set; }
        public string Location { get; set; }

        public virtual ICollection<Department> Department { get; set; }
        public virtual ICollection<DisbursementList> DisbursementList { get; set; }
    }
}
