using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class JournalService : IJournalService
{
    private readonly ReflektDatabase _db;

    public JournalService(ReflektDatabase db) => _db = db;

    public Task<JournalEntry?> GetByDateAsync(string dateIso) =>
        _db.Conn.Table<JournalEntry>().Where(x => x.DateIso == dateIso).FirstOrDefaultAsync();

    public async Task<IReadOnlyList<JournalEntry>> ListAllAsync() =>
        await _db.Conn.Table<JournalEntry>().OrderByDescending(x => x.DateIso).ToListAsync().ConfigureAwait(false);

    public async Task<int> UpsertAsync(JournalEntry entry)
    {
        if (entry.Id == 0)
        {
            entry.CreatedAtUtc = DateTimeOffset.UtcNow;
            entry.UpdatedAtUtc = entry.CreatedAtUtc;
            await _db.Conn.InsertAsync(entry).ConfigureAwait(false);
            return entry.Id;
        }

        entry.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await _db.Conn.UpdateAsync(entry).ConfigureAwait(false);
        return entry.Id;
    }

    public Task DeleteAsync(int entryId) => _db.Conn.DeleteAsync<JournalEntry>(entryId);
}