using Reservation;

namespace ReservationSync;

public class UsernameReservationSync
{
    private TimeSpan Timeout => TimeSpan.FromSeconds(5);
    private readonly FakeDatabase _db;

    public UsernameReservationSync(FakeDatabase db)
    {
        _db = db;
    }

    public bool Reserve(string username)
    {
        if (_db.RegisteredUsernames.Any(x => x == username))
        {
            return false;
        }
        if (_db.ReservedUsernames.Any(x => x == username))
        {
            return false;
        }

        _db.ReservedUsernames.Add(username);

        Task.Run(async () =>
        {
            await Task.Delay(Timeout);
            Expire(username);
        });

        return true;
    }

    private void Expire(string username)
    {
        _db.ReservedUsernames.Remove(username);
    }

    public bool Complete(string username)
    {
        if (_db.ReservedUsernames.Any(x => x == username) == false)
        {
            return false;
        }
        if (_db.RegisteredUsernames.Any(x => x == username))
        {
            return false;
        }

        _db.ReservedUsernames.Remove(username);
        _db.RegisteredUsernames.Add(username);

        return true;
    }
}