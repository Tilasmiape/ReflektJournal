using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class MoodService : IMoodService
{
    private readonly ReflektDatabase _db;

    public MoodService(ReflektDatabase db) => _db = db;

    public async Task<IReadOnlyList<Mood>> ListAsync() =>
        await _db.Conn.Table<Mood>().OrderBy(x => x.Name).ToListAsync().ConfigureAwait(false);
}