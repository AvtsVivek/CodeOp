using httpapi;
using Orleans;
using Orleans.Hosting;
using Polly;

var client = await Policy<IClusterClient>
    .Handle<Exception>()
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(3)
    })
    .ExecuteAsync(async () =>
    {
        var client = new ClientBuilder()
            .UseLocalhostClustering()
            .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IInvoiceNumberGeneratorGrain).Assembly))
            .Build();

        await client.Connect();

        return client;
    });

var builder = Host.CreateDefaultBuilder(args);

/*
builder.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering(11111, 30000);
    siloBuilder.AddMemoryGrainStorage("Demo");
});
*/

builder.ConfigureWebHostDefaults(hostBuilder =>
{
    hostBuilder.UseUrls("http://localhost:6000");
    hostBuilder.UseStartup<Startup>();
});
builder.ConfigureServices(collection =>
{
    collection.AddSingleton(client);
});

var app = builder.Build();
app.Run();
