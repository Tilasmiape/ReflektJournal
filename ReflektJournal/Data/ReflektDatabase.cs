using Android.Nfc;
using Android.Telephony.Data;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Icu.Util.ULocale;
using ReflektJournal.Models;

namespace ReflektJournal.Data
{
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
            await Conn.CreateTableAsync<Category>().ConfigureAwait(false);
            await Conn.CreateTableAsync<Tag>().ConfigureAwait(false);
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
                PinLockEnabled = false,
                PinHashBase64 = null,
                PinSaltBase64 = null,
                PinIterations = null
            }).ConfigureAwait(false);
        }

        private async Task EnsureSeedLookupsAsync()
        {
            // Seed moods (you can adjust to match exact screenshot list later)
            if (await Conn.Table<Mood>().CountAsync().ConfigureAwait(false) == 0)
            {
                var moods = new[]
                {
                new Mood { Name = "Happy" },
                new Mood { Name = "Calm" },
                new Mood { Name = "Neutral" },
                new Mood { Name = "Anxious" },
                new Mood { Name = "Sad" },
                new Mood { Name = "Angry" }
            };

                await Conn.InsertAllAsync(moods).ConfigureAwait(false);
            }

            // Seed categories (adjust later to your prototype)
            if (await Conn.Table<Category>().CountAsync().ConfigureAwait(false) == 0)
            {
                var cats = new[]
                {
                new Category { Name = "Personal" },
                new Category { Name = "Work" },
                new Category { Name = "Family" },
                new Category { Name = "Health" },
                new Category { Name = "Travel" }
            };

                await Conn.InsertAllAsync(cats).ConfigureAwait(false);
            }
        }
    }
}
