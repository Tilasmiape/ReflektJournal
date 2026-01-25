using ReflektJournal.Services.Interfaces;

namespace ReflektJournal.Services;

public sealed class PdfExportService : IPdfExportService
{
    public Task<byte[]> ExportPdfAsync()
        => Task.FromResult(Array.Empty<byte>());
}