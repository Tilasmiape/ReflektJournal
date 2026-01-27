
using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface IJournalService
{
    Task<JournalEntry?> GetByDateAsync(string dateIso);
    Task<List<JournalEntry>> ListAsync(int userId);

    /// <summary>
    /// Insert or Update. If Id=0 but an entry exists for the same (UserId+DateIso),
    /// it updates that entry instead of inserting a duplicate.
    /// Returns entry Id.
    /// </summary>
    Task<int> UpsertAsync(JournalEntry entry);

    Task DeleteAsync(int entryId);
}
