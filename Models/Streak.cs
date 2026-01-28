using SQLite;

namespace ReflektJournal.Models;

[Table("Streaks")]
public class Streak
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public bool EntryCreated { get; set; }

    public int CurrentStreak { get; set; }

    public int LongestStreak { get; set; }
}