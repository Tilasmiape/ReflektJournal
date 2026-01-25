using SQLite;

namespace ReflektJournal.Models;

[Table("users")]
public sealed class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed(Unique = true)]
    public string Email { get; set; } = default!;

    public string Name { get; set; } = default!;

    // Local auth vault fields (PBKDF2)
    public string PasswordSaltBase64 { get; set; } = default!;
    public string PasswordHashBase64 { get; set; } = default!;
    public int PasswordIterations { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}