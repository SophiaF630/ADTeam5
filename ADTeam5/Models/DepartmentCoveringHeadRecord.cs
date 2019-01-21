﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ADTeam5.Models
{
    public partial class DepartmentCoveringHeadRecord
    {
        public int Chindex { get; set; }

        public int UserId { get; set; }
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DateRange]
        public DateTime EndDate { get; set; }

        public virtual User User { get; set; }

        public static implicit operator DepartmentCoveringHeadRecord(User v)
        {
            throw new NotImplementedException();
        }
    }
}
