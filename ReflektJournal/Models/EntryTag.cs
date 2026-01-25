using SQLite;

namespace ReflektJournal.Models;

[Table("entry_tags")]
public sealed class EntryTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int EntryId { get; set; }

    [Indexed]
    public int TagId { get; set; }
}