using NServiceBus;

namespace ReservationSaga;

public class ConfirmUsernameReservation : ICommand
{
    public string Username { get; set; }
}

public class UsernameRegistered : IEvent
{
    public string Username { get; set; }
}

public class ConfirmUsernameReservationHandler : IHandleMessages<ConfirmUsernameReservation>
{
    private readonly UsernameReservation _usernameReservation;

    public ConfirmUsernameReservationHandler(UsernameReservation usernameReservation)
    {
        _usernameReservation = usernameReservation;
    }

    public async Task Handle(ConfirmUsernameReservation message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Async: Confirmed Reservation for {message.Username}");

        if (_usernameReservation.Complete(message.Username))
        {
            await context.Publish(new UsernameRegistered { Username = message.Username });
        }
    }
}