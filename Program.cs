using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SellingSolutions.Models;

namespace SellingSolutions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Identify the host machine
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    // Identify the context(s)
                    var context = services.GetRequiredService<SellingSolutionsContext>();
                    var identityContext = services.GetRequiredService<SellingSolutionsIdentityContext>();

                    // Apply the migration(s)
                    context.Database.Migrate();
                    identityContext.Database.Migrate();

                    // Call and run the 'Initialze' class
                    SeedData.Initialize(services);
                }
                catch (Exception err)
                {
                    // Store the error
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    // Return the error
                    logger.LogError(err, "Seeding into the Database 'Item' has failed");
                }
            }
            // Run the web hosting service
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
