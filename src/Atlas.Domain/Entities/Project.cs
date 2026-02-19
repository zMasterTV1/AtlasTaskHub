namespace Atlas.Domain.Entities;

public sealed class Project
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Optimistic concurrency token.</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public List<TaskItem> Tasks { get; set; } = new();
}
