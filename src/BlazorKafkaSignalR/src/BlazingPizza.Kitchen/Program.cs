using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazingPizza.Kitchen
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:6001") });
            builder.Services.AddHttpClient<OrdersClient>(client => client.BaseAddress = new Uri("https://localhost:6001"));
            builder.Services.AddScoped<OrderState>();

            // Add auth services
            /*
            builder.Services.AddApiAuthorization<PizzaAuthenticationState>(options =>
            {
                options.ProviderOptions.ConfigurationEndpoint = "https://localhost:6001/_configuration/BlazingPizza.Client";
                options.AuthenticationPaths.LogOutSucceededPath = "";
            });
            */

            await builder.Build().RunAsync();
        }
    }
}
