using NServiceBus;

namespace ReservationSaga;

public class UserRegistrationStarted : IEvent
{
    public string Username { get; set; }
}