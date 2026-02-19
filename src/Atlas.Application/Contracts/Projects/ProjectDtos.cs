using Atlas.Domain.Enums;

namespace Atlas.Application.Contracts.Projects;

public sealed record ProjectSummaryDto(Guid Id, string Name, string? Description, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record TaskDto(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateProjectRequest(string Name, string? Description);

public sealed record CreateTaskRequest(string Title, string? Description, DateTimeOffset? DueDate);
