using System;
using System.Collections.Generic;

namespace ADTeam5.Models
{
    public partial class UserAccount
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string LoginPassword { get; set; }
    }
}
