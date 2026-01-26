
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

        // DEV-SAFE RESET: schema evolves early; easiest to reset lookup tables
        await ResetLookupTablesForDevAsync().ConfigureAwait(false);

        await Conn.CreateTableAsync<Mood>().ConfigureAwait(false);
        await Conn.CreateTableAsync<EntryMood>().ConfigureAwait(false);

        await Conn.CreateTableAsync<JournalTag>().ConfigureAwait(false);
        await Conn.CreateTableAsync<EntryTag>().ConfigureAwait(false);

        await Conn.CreateTableAsync<AppSettings>().ConfigureAwait(false);

        await EnsureSingletonSettingsRowAsync().ConfigureAwait(false);
        await EnsureSeedLookupsAsync().ConfigureAwait(false);
    }

    private async Task ResetLookupTablesForDevAsync()
    {
        // Why: sqlite-net CreateTableAsync does not add new columns automatically.
        // Removing these tables is OK early dev; do real migrations later.
        await Conn.ExecuteAsync("DROP TABLE IF EXISTS moods").ConfigureAwait(false);
        await Conn.ExecuteAsync("DROP TABLE IF EXISTS tags").ConfigureAwait(false);
        await Conn.ExecuteAsync("DROP TABLE IF EXISTS entry_moods").ConfigureAwait(false);
        await Conn.ExecuteAsync("DROP TABLE IF EXISTS entry_tags").ConfigureAwait(false);
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
            var moods = new List<Mood>
            {
                // Primary (3)
                new() { Name = "Positive", GroupKey = "Positive", IsPrimary = true },
                new() { Name = "Neutral",  GroupKey = "Neutral",  IsPrimary = true },
                new() { Name = "Negative", GroupKey = "Negative", IsPrimary = true },

                // Secondary - Positive
                new() { Name = "Happy",     GroupKey = "Positive", IsPrimary = false },
                new() { Name = "Excited",   GroupKey = "Positive", IsPrimary = false },
                new() { Name = "Relaxed",   GroupKey = "Positive", IsPrimary = false },
                new() { Name = "Grateful",  GroupKey = "Positive", IsPrimary = false },
                new() { Name = "Confident", GroupKey = "Positive", IsPrimary = false },

                // Secondary - Neutral
                new() { Name = "Calm",       GroupKey = "Neutral", IsPrimary = false },
                new() { Name = "Thoughtful", GroupKey = "Neutral", IsPrimary = false },
                new() { Name = "Curious",    GroupKey = "Neutral", IsPrimary = false },
                new() { Name = "Nostalgic",  GroupKey = "Neutral", IsPrimary = false },
                new() { Name = "Bored",      GroupKey = "Neutral", IsPrimary = false },

                // Secondary - Negative
                new() { Name = "Sad",      GroupKey = "Negative", IsPrimary = false },
                new() { Name = "Angry",    GroupKey = "Negative", IsPrimary = false },
                new() { Name = "Stressed", GroupKey = "Negative", IsPrimary = false },
                new() { Name = "Lonely",   GroupKey = "Negative", IsPrimary = false },
                new() { Name = "Anxious",  GroupKey = "Negative", IsPrimary = false },
            };

            await Conn.InsertAllAsync(moods).ConfigureAwait(false);
        }

        if (await Conn.Table<JournalTag>().CountAsync().ConfigureAwait(false) == 0)
        {
            var tags = new[]
            {
                "Work","Career","Studies","Family","Friends","Relationships",
                "Health","Fitness","Personal Growth","Self-Care","Hobbies","Travel",
                "Nature","Finance","Spirituality","Birthday","Holiday","Vacation"
            };

            await Conn.InsertAllAsync(tags.Select(t => new JournalTag { Name = t })).ConfigureAwait(false);
        }
    }
}