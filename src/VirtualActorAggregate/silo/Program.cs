using System.Net;
using Orleans.Hosting;

var builder = Host.CreateDefaultBuilder(args);
builder.UseOrleans(siloBuilder =>
{
    var port = args.Any() ? Convert.ToInt32(args[0]) : 11111;
    siloBuilder.UseLocalhostClustering(port, 30000, new IPEndPoint(IPAddress.Loopback, 11111));
    siloBuilder.AddMemoryGrainStorage("Demo");
});

var app = builder.Build();
app.Run();