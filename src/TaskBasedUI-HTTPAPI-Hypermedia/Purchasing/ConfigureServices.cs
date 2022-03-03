using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Purchasing
{
    public static class ConfigureServices
    {
        public static void ConfigurePurchasingServices(this IServiceCollection collection)
        {
            collection.AddDbContext<PurchasingDbContext>();
            collection.AddMediatR(typeof(ConfigureServices).Assembly);
        }
    }
}