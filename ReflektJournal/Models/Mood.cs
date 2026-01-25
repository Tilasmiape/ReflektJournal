using SQLite;

namespace ReflektJournal.Models;

[Table("moods")]
public sealed class Mood
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Name { get; set; } = default!;
}