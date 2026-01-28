
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
        // Note: If you want per-user uniqueness, enforce it in Today.razor
        return await _db.Conn.Table<JournalEntry>()
            .Where(x => x.DateIso == dateIso)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    public async Task<JournalEntry?> GetByIdAsync(int id)
    {
        return await _db.Conn.Table<JournalEntry>()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
    }

    // ✅ FIX: return type matches interface (IReadOnlyList)
    public async Task<List<JournalEntry>> ListAsync(int userId)
    {
        var list = await _db.Conn.Table<JournalEntry>()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.DateIso)
            .ThenByDescending(x => x.Id)
            .ToListAsync()
            .ConfigureAwait(false);

        return list;
    }

    public async Task<int> UpsertAsync(JournalEntry entry)
    {
        if (entry.Id == 0)
        {
            await _db.Conn.InsertAsync(entry).ConfigureAwait(false);
            return entry.Id;
        }

        await _db.Conn.UpdateAsync(entry).ConfigureAwait(false);
        return entry.Id;
    }

    public async Task DeleteAsync(int id)
    {
        // ✅ Delete join rows too so filters + lookups stay consistent
        await _db.Conn.RunInTransactionAsync(conn =>
        {
            conn.Execute("DELETE FROM entry_moods WHERE EntryId = ?", id);
            conn.Execute("DELETE FROM entry_tags WHERE EntryId = ?", id);
            conn.Delete<JournalEntry>(id);
        }).ConfigureAwait(false);
    }
}
