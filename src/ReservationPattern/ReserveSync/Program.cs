using Microsoft.Extensions.DependencyInjection;
using Reservation;
using ReservationSync;

var services = new ServiceCollection();
services.AddSingleton<FakeDatabase>();
services.AddTransient<IUserRepository, UserRepository>();
services.AddTransient<UserRegistration>();
services.AddTransient<UsernameReservationSync>();
var provider = services.BuildServiceProvider();

var username = string.Empty;
while (username != "q")
{
    Console.Write("Username: ");
    username = Console.ReadLine();
    var userRegistration = provider.GetRequiredService<UserRegistration>();
    var result = userRegistration.Register(username);
    Console.WriteLine(result ? "Registration Complete" : "Registration Failed");
}