using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADTeam5.ViewModels
{
    public class OutstandingOrder
    {
        [Display(Name = "Request No.")]
        public string Rrid { get; set; }
        [Display(Name = "Requester Name")]
        public string Name { get; set; }
        public string Status { get; set; }


    }
}
