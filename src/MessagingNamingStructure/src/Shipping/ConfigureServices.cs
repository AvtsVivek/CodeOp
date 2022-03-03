using Microsoft.Extensions.DependencyInjection;
using Shipping.Features;

namespace Shipping
{
    public static class ConfigureServices
    {
        public static void AddShipping(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<ShippingDbContext>();
            serviceCollection.AddTransient<CreateShippingLabelHandler>();
        }
    }
}
