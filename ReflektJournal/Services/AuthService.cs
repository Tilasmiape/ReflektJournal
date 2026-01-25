using System.Security.Cryptography;
using System.Text.Json;
using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class AuthService : IAuthService
{
    private const string SessionKey = "reflekt_session_unlocked_v1";
    private readonly ReflektDatabase _db;

    private User? _user;
    private bool _unlocked;

    public bool IsUnlocked => _unlocked;

    public AuthService(ReflektDatabase db) => _db = db;

    public async Task<bool> IsRegisteredAsync()
    {
        var u = await _db.Conn.Table<User>().FirstOrDefaultAsync().ConfigureAwait(false);
        return u is not null;
    }

    public async Task<User?> GetUserAsync()
    {
        if (_user is not null) return _user;
        _user = await _db.Conn.Table<User>().FirstOrDefaultAsync().ConfigureAwait(false);
        return _user;
    }

    public async Task RegisterAsync(string name, string email, string password)
    {
        name = (name ?? "").Trim();
        email = (email ?? "").Trim();

        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new ArgumentException("Valid email is required.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6) throw new ArgumentException("Password must be at least 6 characters.");

        if (await IsRegisteredAsync().ConfigureAwait(false))
            throw new InvalidOperationException("User already exists on this device.");

        var salt = RandomNumberGenerator.GetBytes(16);
        const int iterations = 150_000;
        var hash = DeriveKey(password, salt, iterations, 32);

        var user = new User
        {
            Name = name,
            Email = email,
            PasswordSaltBase64 = Convert.ToBase64String(salt),
            PasswordHashBase64 = Convert.ToBase64String(hash),
            PasswordIterations = iterations,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await _db.Conn.InsertAsync(user).ConfigureAwait(false);

        _user = user;
        _unlocked = true;

        await SecureStorage.SetAsync(SessionKey, JsonSerializer.Serialize(new { unlocked = true })).ConfigureAwait(false);
    }

    public async Task<bool> UnlockAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            return false;

        var user = await GetUserAsync().ConfigureAwait(false);
        if (user is null) return false;

        var salt = Convert.FromBase64String(user.PasswordSaltBase64);
        var expected = Convert.FromBase64String(user.PasswordHashBase64);

        var actual = DeriveKey(password, salt, user.PasswordIterations, expected.Length);
        var ok = CryptographicOperations.FixedTimeEquals(actual, expected);

        _unlocked = ok;
        if (ok)
            await SecureStorage.SetAsync(SessionKey, JsonSerializer.Serialize(new { unlocked = true })).ConfigureAwait(false);

        return ok;
    }

    public Task LockAsync()
    {
        _unlocked = false;
        SecureStorage.Remove(SessionKey);
        return Task.CompletedTask;
    }

    private static byte[] DeriveKey(string password, byte[] salt, int iterations, int length)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(length);
    }
}