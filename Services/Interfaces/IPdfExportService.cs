namespace ReflektJournal.Services.Interfaces;

public interface IPdfExportService
{
    // Commit later: generate Journal Archives PDF
    Task<byte[]> ExportPdfAsync();
}