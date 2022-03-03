using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.Features.PlaceOrder;
using NServiceBus;
using NServiceBus.Encryption.MessageProperty;

namespace Microsoft.eShopWeb.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                        .Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var catalogContext = services.GetRequiredService<CatalogContext>();
                    await CatalogContextSeed.SeedAsync(catalogContext, loggerFactory);

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await AppIdentityDbContextSeed.SeedAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(_ =>
                {
                    var endpointConfiguration = new EndpointConfiguration("eShopOnWebEDA");
                    endpointConfiguration.UsePersistence<LearningPersistence>();
                    endpointConfiguration.EnableInstallers();
                    endpointConfiguration.EnableMessagePropertyEncryption(new RijndaelEncryptionService("2021-11", Convert.FromBase64String("gdDbqRpqdRbTs3mhdZh9qCaDaxJXl+e6")));

                    var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
                    transport.ConnectionString("host=localhost");
                    transport.UseConventionalRoutingTopology();

                    var routing = transport.Routing();

                    routing.RouteToEndpoint(
                        assembly: typeof(OrderPlacedEvent).Assembly,
                        destination: "eShopOnWebEDA");

                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
