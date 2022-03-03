using NServiceBus;

namespace ReservationSaga;

public class ExpireReservation : ICommand
{
    public string Username { get; set; }
}

public class ExpireReservationHandler : IHandleMessages<ExpireReservation>
{
    private readonly UsernameReservation _usernameReservation;

    public ExpireReservationHandler(UsernameReservation usernameReservation)
    {
        _usernameReservation = usernameReservation;
    }

    public Task Handle(ExpireReservation message, IMessageHandlerContext context)
    {
        if (_usernameReservation.IsReserved(message.Username))
        {
            Console.WriteLine($"Async: Expire Reservation for {message.Username}");
            _usernameReservation.Expire(message.Username);
        }
        return Task.CompletedTask;
    }
}