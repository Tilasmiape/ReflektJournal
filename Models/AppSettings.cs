using SQLite;

namespace ReflektJournal.Models;

[Table("app_settings")]
public sealed class AppSettings
{
    [PrimaryKey]
    public int Id { get; set; }

    public string ThemeMode { get; set; } = "Dark";

    public bool PinLockEnabled { get; set; }

    public string? PinHashBase64 { get; set; }
    public string? PinSaltBase64 { get; set; }
    public int? PinIterations { get; set; }
}