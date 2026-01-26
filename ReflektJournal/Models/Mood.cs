
using SQLite;

namespace ReflektJournal.Models;

[Table("moods")]
public sealed class Mood
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public string Name { get; set; } = default!;

    // "Positive" | "Neutral" | "Negative"
    [Indexed]
    public string GroupKey { get; set; } = default!;

    // true for the 3 primary moods (Positive/Neutral/Negative)
    [Indexed]
    public bool IsPrimary { get; set; }
}
