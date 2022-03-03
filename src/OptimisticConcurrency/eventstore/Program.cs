using EventSourcing.Demo;
using EventStore.ClientAPI.Exceptions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/products/{sku}", async (HttpResponse response, string sku) =>
{
    using var stream = await WarehouseProductEventStoreStream.Factory();
    var product = await stream.Get(sku);
    //response.Headers.ETag = product.Version.ToString();

    return new
    {
        Sku = product.Aggregate.Sku,
        Quantity = product.Aggregate.GetQuantityOnHand(),
        Version = product.Version,
        Commands = new Command[]
        {
            new("InventoryAdjustment", $"/products/{sku}/{product.Version}/adjustment")
        }
    };
});

app.MapPost("/products/{sku}/adjustment", async (HttpRequest request, HttpResponse response, [FromRoute]string sku, [FromBody]InventoryAdjustment inventoryAdjustment) =>
{
    using var stream = await WarehouseProductEventStoreStream.Factory();
    var product = await stream.Get(sku);
    product.Aggregate.AdjustInventory(inventoryAdjustment.Quantity, inventoryAdjustment.Reason);

    var eTag = request.GetIfMatch();
    if (eTag != null)
    {
        try
        {
            await stream.Save(product.Aggregate, eTag.Value);
            response.StatusCode = 204;
            return;
        }
        catch (WrongExpectedVersionException)
        {
            response.StatusCode = 412;
        }
    }

    response.StatusCode = 412;
});

app.MapPost("/products/{sku}/{version}/adjustment",
    async (HttpResponse response, [FromRoute]string sku, [FromRoute]long version, [FromBody]InventoryAdjustment inventoryAdjustment) =>
{
    using var stream = await WarehouseProductEventStoreStream.Factory();
    var product = await stream.Get(sku);
    product.Aggregate.AdjustInventory(inventoryAdjustment.Quantity, inventoryAdjustment.Reason);
    try
    {
        await stream.Save(product.Aggregate, version);
    }
    catch (WrongExpectedVersionException)
    {
        response.StatusCode = 412;
    }
});

app.Run();

public class InventoryAdjustment
{
    public int Quantity { get; set; }
    public string Reason { get; set; }
}

public static class Extensions
{
    public static long? GetIfMatch(this HttpRequest request)
    {
        var headers = request.GetTypedHeaders();
        var eTag = headers.IfMatch.FirstOrDefault()?.Tag.Value.Replace("\"", "");
        if (eTag != null && long.TryParse(eTag, out var version))
        {
            return version;
        }

        return null;
    }
}

public class Command
{
    public Command(string action, string uri)
    {
        Action = action;
        Uri = uri;
    }

    public string Action { get; set; }
    public string Uri { get; set; }
}
