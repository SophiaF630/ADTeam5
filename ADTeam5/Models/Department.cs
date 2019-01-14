using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class Department
    {
        public Department()
        {
            User = new HashSet<User>();
        }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int CollectionPointId { get; set; }
        public int HeadId { get; set; }
        public int RepId { get; set; }
        public int? CoveringHeadId { get; set; }
        public string CollectionPassword { get; set; }

        public virtual CollectionPoint CollectionPoint { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
