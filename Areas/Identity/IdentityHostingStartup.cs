using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SellingSolutions.Areas.Identity.Data;
using SellingSolutions.Models;

[assembly: HostingStartup(typeof(SellingSolutions.Areas.Identity.IdentityHostingStartup))]
namespace SellingSolutions.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<SellingSolutionsIdentityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("SellingSolutionsIdentityContextConnection")));

                services.AddDefaultIdentity<SellingSolutionsUser>()
                    .AddEntityFrameworkStores<SellingSolutionsIdentityContext>()
                    .AddDefaultTokenProviders();
            });
        }
    }
}