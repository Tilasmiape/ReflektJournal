using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface IJournalService
{
    Task<JournalEntry?> GetByDateAsync(string dateIso);
    Task<JournalEntry?> GetByIdAsync(int id);
    Task<List<JournalEntry>> ListAsync(int userId);
    Task<int> UpsertAsync(JournalEntry entry);
    Task DeleteAsync(int entryId);
}
