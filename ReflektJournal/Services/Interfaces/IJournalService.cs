using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface IJournalService
{
    Task<JournalEntry?> GetByDateAsync(string dateIso);
    Task<IReadOnlyList<JournalEntry>> ListAllAsync();
    Task<int> UpsertAsync(JournalEntry entry);
    Task DeleteAsync(int entryId);
}