using SQLite;

namespace ReflektJournal.Models;

[Table("journal_entries")]
public sealed class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UserId { get; set; }

    // yyyy-MM-dd (enforces “one entry per day” easily)
    [Indexed(Unique = true)]
    public string DateIso { get; set; } = default!;

    public string Title { get; set; } = default!;
    public string ContentMarkdown { get; set; } = default!;

    public int? CategoryId { get; set; }
    public int WordCount { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
}