using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface IMoodService
{
    Task<IReadOnlyList<Mood>> ListAsync();
}