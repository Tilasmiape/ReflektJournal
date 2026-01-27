using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class MoodService : IMoodService
{
    private readonly ReflektDatabase _db;
    public MoodService(ReflektDatabase db) => _db = db;

    public async Task<IReadOnlyList<Mood>> ListAsync()
    {
        await _db.InitializeAsync().ConfigureAwait(false);
        return await _db.Conn.Table<Mood>().OrderBy(x => x.Name).ToListAsync().ConfigureAwait(false);
    }

    public async Task SetEntryMoodsAsync(int entryId, int primaryMoodId, IReadOnlyList<int> secondaryMoodIds)
    {
        await _db.InitializeAsync().ConfigureAwait(false);

        var secondary = secondaryMoodIds
            .Distinct()
            .Where(x => x != primaryMoodId)
            .Take(2)
            .ToList();

        await _db.Conn.RunInTransactionAsync(conn =>
        {
            conn.Execute("DELETE FROM entry_moods WHERE EntryId = ?", entryId);

            conn.Insert(new EntryMood
            {
                EntryId = entryId,
                MoodId = primaryMoodId,
                Kind = "Primary"
            });

            foreach (var moodId in secondary)
            {
                conn.Insert(new EntryMood
                {
                    EntryId = entryId,
                    MoodId = moodId,
                    Kind = "Secondary"
                });
            }
        }).ConfigureAwait(false);
    }

    public async Task<(int? PrimaryMoodId, IReadOnlyList<int> SecondaryMoodIds)> GetEntryMoodsAsync(int entryId)
    {
        await _db.InitializeAsync().ConfigureAwait(false);

        var rows = await _db.Conn.Table<EntryMood>()
            .Where(x => x.EntryId == entryId)
            .ToListAsync()
            .ConfigureAwait(false);

        var primary = rows.FirstOrDefault(r => r.Kind == "Primary")?.MoodId;
        var secondary = rows.Where(r => r.Kind == "Secondary").Select(r => r.MoodId).ToList();
        return (primary, secondary);
    }
}
