using Atlas.Domain.Entities;

namespace Atlas.Domain.Entities;

public sealed class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public string PasswordSalt { get; set; } = default!;

    public string Role { get; set; } = "User";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<RefreshToken> RefreshTokens { get; set; } = new();

    public List<Project> Projects { get; set; } = new();
}
