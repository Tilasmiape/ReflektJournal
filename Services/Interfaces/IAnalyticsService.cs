namespace ReflektJournal.Services.Interfaces;

public interface IAnalyticsService
{
    // Commit later: return analytics DTOs
    Task<object> GetSnapshotAsync();
}