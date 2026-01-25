using SQLite;

namespace ReflektJournal.Models;

[Table("tags")]
public sealed class JournalTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Name { get; set; } = default!;
}
