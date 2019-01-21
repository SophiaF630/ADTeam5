using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ADTeam5.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ADTeam5User class
    public class ADTeam5User : IdentityUser
    {
        [PersonalData]
        public int WorkID { get; set; }
    }
}
