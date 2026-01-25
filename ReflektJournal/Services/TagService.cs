using ReflektJournal.Data;
using ReflektJournal.Models;
using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class TagService : ITagService
{
    private readonly ReflektDatabase _db;

    public TagService(ReflektDatabase db) => _db = db;

    public async Task<IReadOnlyList<Tag>> ListAsync() =>
        await _db.Conn.Table<Tag>().OrderBy(x => x.Name).ToListAsync().ConfigureAwait(false);

    public async Task SetTagsForEntryAsync(int entryId, IReadOnlyList<string> tagNames)
    {
        var normalized = tagNames
            .Select(t => (t ?? "").Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        await _db.Conn.RunInTransactionAsync(conn =>
        {
            conn.Execute("DELETE FROM entry_tags WHERE EntryId = ?", entryId);
        }).ConfigureAwait(false);

        foreach (var name in normalized)
        {
            var tag = await _db.Conn.Table<Tag>().Where(t => t.Name == name).FirstOrDefaultAsync().ConfigureAwait(false);
            if (tag is null)
            {
                tag = new Tag { Name = name };
                await _db.Conn.InsertAsync(tag).ConfigureAwait(false);
            }

            await _db.Conn.InsertAsync(new EntryTag { EntryId = entryId, TagId = tag.Id }).ConfigureAwait(false);
        }
    }

    public async Task<IReadOnlyList<string>> ListTagsForEntryAsync(int entryId)
    {
        var rows = await _db.Conn.QueryAsync<string>(
            @"SELECT t.Name
              FROM tags t
              INNER JOIN entry_tags et ON et.TagId = t.Id
              WHERE et.EntryId = ?",
            entryId).ConfigureAwait(false);

        return rows;
    }
}