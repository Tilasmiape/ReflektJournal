
using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface IMoodService
{
    Task<IReadOnlyList<Mood>> ListAsync();

    Task SetEntryMoodsAsync(int entryId, int primaryMoodId, IReadOnlyList<int> secondaryMoodIds);
    Task<(int? PrimaryMoodId, IReadOnlyList<int> SecondaryMoodIds)> GetEntryMoodsAsync(int entryId);
}
