using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public sealed class TaskItem
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Project Project { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    public DateTimeOffset? DueDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
