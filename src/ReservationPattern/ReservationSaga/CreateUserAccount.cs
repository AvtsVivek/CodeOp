using NServiceBus;

namespace ReservationSaga;

public class CreateUserAccount : ICommand
{
    public string Username { get; set; }
}

public class CreateUserAccountHandler : IHandleMessages<CreateUserAccount>
{
    public async Task Handle(CreateUserAccount message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Async: Create User Account for {message.Username}");

        await context.Publish(new UserAccountCreated { Username = message.Username });
    }
}

public class UserAccountCreated : IEvent
{
    public string Username { get; set; }
}