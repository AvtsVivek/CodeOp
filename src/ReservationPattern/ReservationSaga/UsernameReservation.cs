using Reservation;

namespace ReservationSaga;

public class UsernameReservation
{
    private readonly FakeDatabase _db;

    public UsernameReservation(FakeDatabase db)
    {
        _db = db;
    }

    public bool IsAvailable(string username)
    {
        return _db.RegisteredUsernames.Contains(username) == false && _db.ReservedUsernames.Contains(username) == false;
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
        return true;
    }

    public void Expire(string username)
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

    public bool IsReserved(string username)
    {
        return _db.ReservedUsernames.Contains(username);
    }
}