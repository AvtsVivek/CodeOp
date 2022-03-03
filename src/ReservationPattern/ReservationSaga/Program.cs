using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Reservation;
using ReservationSaga;

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseNServiceBus(context =>
        {
            var endpointConfiguration = new EndpointConfiguration("Demo");
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();

            var routing = transport.Routing();

            routing.RouteToEndpoint(
                assembly: typeof(UserRegistration).Assembly,
                destination: "Demo");

            return endpointConfiguration;
        })
        .ConfigureServices(services =>
        {
            services.AddSingleton<FakeDatabase>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<UsernameReservation>();
        });

var host = CreateHostBuilder(args).Build();
await host.StartAsync();

var session = host.Services.GetRequiredService<IMessageSession>();
var usernameReservation = host.Services.GetRequiredService<UsernameReservation>();

var username = string.Empty;
while (username != "q")
{
    Console.Write("Username: ");
    username = Console.ReadLine();
    if (usernameReservation.IsAvailable(username))
    {
        await session.Publish(new UserRegistrationStarted { Username = username });
    }
    else
    {
        Console.WriteLine("Username already exists.");
    }
    Console.ReadLine();
}