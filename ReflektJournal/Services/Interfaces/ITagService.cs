using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface ITagService
{
    Task<IReadOnlyList<Tag>> ListAsync();
    Task SetTagsForEntryAsync(int entryId, IReadOnlyList<string> tagNames);
    Task<IReadOnlyList<string>> ListTagsForEntryAsync(int entryId);
}