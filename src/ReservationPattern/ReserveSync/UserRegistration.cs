using Reservation;

namespace ReservationSync;

public class UserRegistration
{
    private static int _testCount = 0;
    private readonly UsernameReservationSync _reservation;
    private readonly IUserRepository _repository;

    public UserRegistration(UsernameReservationSync reservation, IUserRepository repository)
    {
        _reservation = reservation;
        _repository = repository;
    }

    public bool Register(string username)
    {
        if (_reservation.Reserve(username) == false)
        {
            return false;
        }

        // For testing to show the expiry
        if (username == "test" && _testCount == 0)
        {
            _testCount++;
            return false;
        }

        var account = new Account(username);
        _repository.Add(account);
        _repository.Save();

        if (_reservation.Complete(username) == false)
        {
            _repository.Remove(account);
            return false;
        }

        return true;
    }
}