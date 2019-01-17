using System;
using ADTeam5.Areas.Identity.Data;
using ADTeam5.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ADTeam5.Areas.Identity.IdentityHostingStartup))]
namespace ADTeam5.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                //services.AddDbContext<ADTeam5UserContext>(options =>
                //    options.UseSqlServer(
                //        context.Configuration.GetConnectionString("ADTeam5UserContextConnection")));

                //services.AddDefaultIdentity<ADTeam5User>()
                //    .AddEntityFrameworkStores<ADTeam5UserContext>();
            });
        }
    }
}