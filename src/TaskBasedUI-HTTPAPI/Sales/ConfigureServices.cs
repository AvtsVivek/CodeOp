using Microsoft.Extensions.DependencyInjection;

namespace Sales
{
    public static class ConfigureServices
    {
        public static void ConfigureSalesServices(this IServiceCollection collection)
        {
            collection.AddDbContext<SalesDbContext>();
        }

        public static void AddSalesControllers(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddApplicationPart(typeof(ConfigureServices).Assembly);
        }
    }
}