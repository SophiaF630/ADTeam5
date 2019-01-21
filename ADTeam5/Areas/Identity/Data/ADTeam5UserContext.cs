using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADTeam5.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ADTeam5.Models
{
    public class ADTeam5UserContext : IdentityDbContext<ADTeam5User>
    {
        public ADTeam5UserContext(DbContextOptions<ADTeam5UserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
