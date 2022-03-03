using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog
{
    public static class ConfigureServices
    {
        public static void ConfigureCatalogServices(this IServiceCollection collection)
        {
            collection.AddDbContext<CatalogDbContext>();
            collection.AddMediatR(typeof(ConfigureServices).Assembly);
        }

        public static void AddCatalogControllers(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(typeof(ConfigureServices).Assembly);
        }
    }
}