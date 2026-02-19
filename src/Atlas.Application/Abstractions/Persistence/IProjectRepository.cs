using Atlas.Domain.Entities;

namespace Atlas.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<Project?> GetProjectAsync(Guid projectId, Guid ownerId, CancellationToken ct);
    Task<IReadOnlyList<Project>> ListProjectsAsync(Guid ownerId, int skip, int take, CancellationToken ct);
    Task<int> CountProjectsAsync(Guid ownerId, CancellationToken ct);

    Task<IReadOnlyList<TaskItem>> ListTasksAsync(Guid projectId, Guid ownerId, int skip, int take, string? q, CancellationToken ct);
    Task<int> CountTasksAsync(Guid projectId, Guid ownerId, string? q, CancellationToken ct);

    void AddProject(Project project);
    void AddTask(TaskItem task);
}
