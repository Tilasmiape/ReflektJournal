
using ReflektJournal.Models;
using SQLite;

namespace ReflektJournal.Data;

public sealed class ReflektDatabase
{
    private readonly Lazy<SQLiteAsyncConnection> _lazyConn;

    public ReflektDatabase()
    {
        _lazyConn = new Lazy<SQLiteAsyncConnection>(() =>
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "reflekt.db3");
            Directory.CreateDirectory(FileSystem.AppDataDirectory);

            return new SQLiteAsyncConnection(
                dbPath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        });
    }

    public SQLiteAsyncConnection Conn => _lazyConn.Value;

    public async Task InitializeAsync()
    {
        await Conn.CreateTableAsync<User>().ConfigureAwait(false);
        await Conn.CreateTableAsync<JournalEntry>().ConfigureAwait(false);
        await Conn.CreateTableAsync<Mood>().ConfigureAwait(false);
        await Conn.CreateTableAsync<EntryCategory>().ConfigureAwait(false);
        await Conn.CreateTableAsync<JournalTag>().ConfigureAwait(false);
        await Conn.CreateTableAsync<EntryTag>().ConfigureAwait(false);
        await Conn.CreateTableAsync<EntryMood>().ConfigureAwait(false);
        await Conn.CreateTableAsync<AppSettings>().ConfigureAwait(false);

        await EnsureSingletonSettingsRowAsync().ConfigureAwait(false);
        await EnsureSeedLookupsAsync().ConfigureAwait(false);
    }

    private async Task EnsureSingletonSettingsRowAsync()
    {
        var row = await Conn.Table<AppSettings>().FirstOrDefaultAsync().ConfigureAwait(false);
        if (row is not null) return;

        await Conn.InsertAsync(new AppSettings
        {
            Id = 1,
            ThemeMode = "Dark",
            PinLockEnabled = false
        }).ConfigureAwait(false);
    }

    private async Task EnsureSeedLookupsAsync()
    {
        if (await Conn.Table<Mood>().CountAsync().ConfigureAwait(false) == 0)
        {
            await Conn.InsertAllAsync(new[]
            {
                new Mood { Name = "Happy" },
                new Mood { Name = "Calm" },
                new Mood { Name = "Neutral" },
                new Mood { Name = "Anxious" },
                new Mood { Name = "Sad" },
                new Mood { Name = "Angry" },
            }).ConfigureAwait(false);
        }

        if (await Conn.Table<EntryCategory>().CountAsync().ConfigureAwait(false) == 0)
        {
            await Conn.InsertAllAsync(new[]
            {
                new EntryCategory { Name = "Personal" },
                new EntryCategory { Name = "Work" },
                new EntryCategory { Name = "Family" },
                new EntryCategory { Name = "Health" },
                new EntryCategory { Name = "Travel" },
            }).ConfigureAwait(false);
        }
    }
}
