
using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class JournalService : IJournalService
{
    private readonly ReflektDatabase _db;

    public JournalService(ReflektDatabase db) => _db = db;

    public async Task<JournalEntry?> GetByDateAsync(string dateIso)
    {
        await _db.InitializeAsync().ConfigureAwait(false);

        // Single-user device scenario: DateIso is enough
        return await _db.Conn.Table<JournalEntry>()
            .Where(e => e.DateIso == dateIso)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<List<JournalEntry>> ListAsync(int userId)
    {
        await _db.InitializeAsync().ConfigureAwait(false);

        var list = await _db.Conn.Table<JournalEntry>()
            .Where(e => e.UserId == userId)
            .ToListAsync()
            .ConfigureAwait(false);

        return list
            .OrderByDescending(e => e.DateIso)
            .ThenByDescending(e => e.Id)
            .ToList();
    }

    public async Task<int> UpsertAsync(JournalEntry entry)
    {
        await _db.InitializeAsync().ConfigureAwait(false);

        if (entry.Id > 0)
        {
            await _db.Conn.UpdateAsync(entry).ConfigureAwait(false);
            return entry.Id;
        }

        // If Id is not set, update by (UserId + DateIso) if exists
        var existing = await _db.Conn.Table<JournalEntry>()
            .Where(e => e.UserId == entry.UserId && e.DateIso == entry.DateIso)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (existing is not null)
        {
            entry.Id = existing.Id;
            await _db.Conn.UpdateAsync(entry).ConfigureAwait(false);
            return entry.Id;
        }

        await _db.Conn.InsertAsync(entry).ConfigureAwait(false);
        return entry.Id;
    }

    public async Task DeleteAsync(int entryId)
    {
        await _db.InitializeAsync().ConfigureAwait(false);

        // Delete join rows safely using sqlite-net types (no column-name guessing)
        var moods = await _db.Conn.Table<EntryMood>()
            .Where(x => x.EntryId == entryId)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var m in moods)
            await _db.Conn.DeleteAsync(m).ConfigureAwait(false);

        var tags = await _db.Conn.Table<EntryTag>()
            .Where(x => x.EntryId == entryId)
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var t in tags)
            await _db.Conn.DeleteAsync(t).ConfigureAwait(false);

        await _db.Conn.DeleteAsync<JournalEntry>(entryId).ConfigureAwait(false);
    }
}
