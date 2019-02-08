using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class BriefEmpReq
    {
        public string rrid { get; set; }

        [DataType(DataType.Date)]
        public DateTime requestDate { get; set; }
        public int userid { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public string remark { get; set; }
    }
}
