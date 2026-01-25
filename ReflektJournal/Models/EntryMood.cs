using SQLite;

namespace ReflektJournal.Models;

[Table("entry_moods")]
public sealed class EntryMood
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EntryId { get; set; }

    [Indexed]
    public int MoodId { get; set; }

    // "Primary" or "Secondary"
    public string Kind { get; set; } = "Primary";
}