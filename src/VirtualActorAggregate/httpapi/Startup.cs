using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace httpapi;

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPost("/invoice/{tenantId}",
                async ([FromServices]IClusterClient clusterClient, [FromRoute]Guid tenantId) =>
            {
                var invoiceNumberGeneratorGrain = clusterClient
                    .GetGrain<IInvoiceNumberGeneratorGrain>(tenantId);

                return await invoiceNumberGeneratorGrain.ReserveInvoiceNumber();
            });
        });
    }
}