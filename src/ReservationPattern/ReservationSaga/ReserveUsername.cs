using NServiceBus;

namespace ReservationSaga;

public class ReserveUsername : ICommand
{
    public string Username { get; set; }
}

public class ReserveUsernameHandler : IHandleMessages<ReserveUsername>
{
    private readonly UsernameReservation _usernameReservation;

    public ReserveUsernameHandler(UsernameReservation usernameReservation)
    {
        _usernameReservation = usernameReservation;
    }

    public async Task Handle(ReserveUsername message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Async: Reserving Username for {message.Username}");

        if (_usernameReservation.Reserve(message.Username))
        {
            var expireOptions = new SendOptions();
            expireOptions.DelayDeliveryWith(TimeSpan.FromSeconds(10));
            await context.Send(new ExpireReservation { Username = message.Username }, expireOptions);

            await context.Publish(new UsernameReserved { Username = message.Username });
        }
    }
}

public class UsernameReserved : IEvent
{
    public string Username { get; set; }
}