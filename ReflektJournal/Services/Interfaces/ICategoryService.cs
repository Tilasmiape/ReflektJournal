using ReflektJournal.Models;

namespace ReflektJournal.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<Category>> ListAsync();
}