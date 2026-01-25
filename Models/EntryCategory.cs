using SQLite;

namespace ReflektJournal.Models;

[Table("categories")]
public sealed class EntryCategory
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Name { get; set; } = default!;
}