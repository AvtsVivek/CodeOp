using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse
{
    public static class ConfigureServices
    {
        public static void ConfigureWarehouseServices(this IServiceCollection collection)
        {
            collection.AddDbContext<WarehouseDbContext>();
            collection.AddMediatR(typeof(ConfigureServices).Assembly);
        }

        public static void AddWarehouseControllers(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(typeof(ConfigureServices).Assembly);
        }
    }
}