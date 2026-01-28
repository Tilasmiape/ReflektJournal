using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReflektJournal.Services.Interfaces;

public interface IPdfExportService
{
    Task<string> ExportTimelineAsync(IReadOnlyList<ExportCard> cards);

    public sealed class ExportCard
    {
        public string DateIso { get; init; } = "";
        public string Title { get; init; } = "";
        public string Mood { get; init; } = "";
        public IReadOnlyList<string> Tags { get; init; } = [];
        public string ContentMarkdown { get; init; } = "";
    }
}
