using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface IAuthService
{
    bool IsUnlocked { get; }

    Task<bool> IsRegisteredAsync();
    Task<User?> GetUserAsync();

    Task RegisterAsync(string name, string email, string password);
    Task<bool> UnlockAsync(string password);
    Task LockAsync();
}