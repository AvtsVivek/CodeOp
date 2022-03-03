using NServiceBus;
namespace ReservationSaga;

public class UserRegistration :
    Saga<EmailReservationSagaData>,
    IAmStartedByMessages<UserRegistrationStarted>,
    IHandleMessages<UsernameReserved>,
    IHandleMessages<UserAccountCreated>,
    IHandleMessages<UsernameRegistered>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<EmailReservationSagaData> mapper)
    {
        mapper.ConfigureMapping<UserRegistrationStarted>(message => message.Username).ToSaga(sagaData => sagaData.Username);
        mapper.ConfigureMapping<UsernameReserved>(message => message.Username).ToSaga(sagaData => sagaData.Username);
        mapper.ConfigureMapping<UserAccountCreated>(message => message.Username).ToSaga(sagaData => sagaData.Username);
        mapper.ConfigureMapping<UsernameRegistered>(message => message.Username).ToSaga(sagaData => sagaData.Username);
    }

    public async Task Handle(UserRegistrationStarted message, IMessageHandlerContext context)
    {
        await context.Send(new ReserveUsername { Username = message.Username });
    }

    public async Task Handle(UsernameReserved message, IMessageHandlerContext context)
    {
        await context.Send(new CreateUserAccount { Username = message.Username });
    }

    public async Task Handle(UserAccountCreated message, IMessageHandlerContext context)
    {
        if (message.Username == "test" && Data.Attempts == 0)
        {
            Data.Attempts++;
            Console.WriteLine("Async: Account Creation Failed.");
            return;
        }

        await context.Send(new ConfirmUsernameReservation { Username = message.Username });
    }

    public Task Handle(UsernameRegistered message, IMessageHandlerContext context)
    {
        Console.WriteLine($"Async: Registration Complete for {message.Username}");

        MarkAsComplete();
        return Task.CompletedTask;
    }
}

public class EmailReservationSagaData : ContainSagaData
{
    public string Username { get; set; }
    public int Attempts { get; set; }
}