using Atlas.Application.Abstractions.Persistence;
using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly AtlasDbContext _db;

    public ProjectRepository(AtlasDbContext db) => _db = db;

    public Task<Project?> GetProjectAsync(Guid projectId, Guid ownerId, CancellationToken ct)
        => _db.Projects.SingleOrDefaultAsync(p => p.Id == projectId && p.OwnerId == ownerId, ct);

    public async Task<IReadOnlyList<Project>> ListProjectsAsync(Guid ownerId, int skip, int take, CancellationToken ct)
        => await _db.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == ownerId)
            .OrderByDescending(p => p.UpdatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);

    public Task<int> CountProjectsAsync(Guid ownerId, CancellationToken ct)
        => _db.Projects.CountAsync(p => p.OwnerId == ownerId, ct);

    public async Task<IReadOnlyList<TaskItem>> ListTasksAsync(Guid projectId, Guid ownerId, int skip, int take, string? q, CancellationToken ct)
    {
        var query = _db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.Project.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(t => t.Title.Contains(q) || (t.Description != null && t.Description.Contains(q)));
        }

        return await query
            .OrderBy(t => t.Status)
            .ThenByDescending(t => t.UpdatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(ct);
    }

    public Task<int> CountTasksAsync(Guid projectId, Guid ownerId, string? q, CancellationToken ct)
    {
        var query = _db.Tasks.Where(t => t.ProjectId == projectId && t.Project.OwnerId == ownerId);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(t => t.Title.Contains(q) || (t.Description != null && t.Description.Contains(q)));

        return query.CountAsync(ct);
    }

    public void AddProject(Project project) => _db.Projects.Add(project);

    public void AddTask(TaskItem task) => _db.Tasks.Add(task);
}
