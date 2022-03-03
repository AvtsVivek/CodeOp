using System.Linq;
using BlazingPizza.Server.Customer;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BlazingPizza.Server
{
    public class Startup
    {
        readonly string BlazingKitchenOrigins = "BlazingKitchen";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCap(x =>
            {
                x.UseKafka("localhost");
                x.UseMySql("Server=localhost;Port=6306;Database=BlazingPizza;Uid=root;Pwd=root;");
            });

            services.Scan(scan => scan
                .FromAssemblyOf<Startup>()
                .AddClasses(classes => classes.AssignableTo<ICapSubscribe>())
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddCors(options =>
            {
                options.AddPolicy(BlazingKitchenOrigins, builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                });
            });

            services.AddSignalR();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] {"application/octet-stream"});
            });

            services.AddDbContext<PizzaStoreContext>(options => options.UseSqlite("Data Source=pizza.db"));

                services.AddDefaultIdentity<PizzaStoreUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<PizzaStoreContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<PizzaStoreUser, PizzaStoreContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPostConfigureOptions<JwtBearerOptions>,
                    ConfigureJwtBearerOptions>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            app.UseCors(BlazingKitchenOrigins);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseCapDashboard();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<CustomerOrderHub>("/customer/orderhub");
                endpoints.MapHub<KitchenOrderHub>("/kitchen/orderhub");
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
