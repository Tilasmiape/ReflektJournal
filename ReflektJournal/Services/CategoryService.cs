
using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ReflektDatabase _db;

    public CategoryService(ReflektDatabase db) => _db = db;

    public async Task<IReadOnlyList<EntryCategory>> ListAsync() =>
        await _db.Conn.Table<EntryCategory>().OrderBy(x => x.Name).ToListAsync().ConfigureAwait(false);
}
