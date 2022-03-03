namespace Reservation;

public class FakeDatabase
{
    public List<string> RegisteredUsernames { get; set; } = new();
    public List<string> ReservedUsernames { get; set; } = new();
    public List<Account> UserAccounts { get; set; } = new();
}

public record Account(string Username);

public interface IUserRepository
{
    public void Add(Account account);
    public void Remove(Account account);
    void Save();
}

public class UserRepository : IUserRepository
{
    private readonly FakeDatabase _db;
    private readonly List<Account> _unsaved = new();

    public UserRepository(FakeDatabase db)
    {
        _db = db;
    }

    public void Add(Account account)
    {
        _unsaved.Add(account);
    }

    public void Remove(Account account)
    {
        var item = _db.UserAccounts.SingleOrDefault(x => x.Username == account.Username);
        if (item != null)
        {
            _db.UserAccounts.Remove(item);
        }
    }

    public void Save()
    {
        _db.UserAccounts.AddRange(_unsaved);
        _unsaved.Clear();
    }
}