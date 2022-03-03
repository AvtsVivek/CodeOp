using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog
{
    public static class ConfigureServices
    {
        public static void ConfigureCatalogServices(this IServiceCollection collection)
        {
            collection.AddDbContext<CatalogDbContext>();
        }

        public static void AddCatalogControllers(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(typeof(ConfigureServices).Assembly);
        }
    }
}