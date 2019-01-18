using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class DepartmentCoveringHeadRecord
    {
        public int Chindex { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual User User { get; set; }
    }
}
