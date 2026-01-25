using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class AnalyticsService : IAnalyticsService
{
    public Task<object> GetSnapshotAsync()
        => Task.FromResult<object>(new { });
}