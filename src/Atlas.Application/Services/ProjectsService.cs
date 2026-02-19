using Atlas.Application.Abstractions.Persistence;
using Atlas.Application.Abstractions.Time;
using Atlas.Application.Common;
using Atlas.Application.Contracts.Projects;
using Atlas.Domain.Entities;

namespace Atlas.Application.Services;

public sealed class ProjectsService
{
    private readonly IProjectRepository _projects;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public ProjectsService(IProjectRepository projects, IUnitOfWork uow, IClock clock)
    {
        _projects = projects;
        _uow = uow;
        _clock = clock;
    }

    public async Task<PagedResult<ProjectSummaryDto>> ListProjectsAsync(Guid ownerId, int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var skip = (page - 1) * pageSize;
        var total = await _projects.CountProjectsAsync(ownerId, ct);
        var items = await _projects.ListProjectsAsync(ownerId, skip, pageSize, ct);

        var dtos = items.Select(p => new ProjectSummaryDto(p.Id, p.Name, p.Description, p.CreatedAt, p.UpdatedAt)).ToList();
        return new PagedResult<ProjectSummaryDto>(dtos, page, pageSize, total);
    }

    public async Task<ProjectSummaryDto> CreateProjectAsync(Guid ownerId, CreateProjectRequest req, CancellationToken ct)
    {
        var name = req.Name?.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Project name is required.");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name,
            Description = req.Description?.Trim(),
            CreatedAt = _clock.UtcNow,
            UpdatedAt = _clock.UtcNow
        };

        _projects.AddProject(project);
        await _uow.SaveChangesAsync(ct);

        return new ProjectSummaryDto(project.Id, project.Name, project.Description, project.CreatedAt, project.UpdatedAt);
    }

    public async Task<PagedResult<TaskDto>> ListTasksAsync(Guid ownerId, Guid projectId, int page, int pageSize, string? q, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var skip = (page - 1) * pageSize;
        q = string.IsNullOrWhiteSpace(q) ? null : q.Trim();

        var total = await _projects.CountTasksAsync(projectId, ownerId, q, ct);
        var items = await _projects.ListTasksAsync(projectId, ownerId, skip, pageSize, q, ct);

        var dtos = items.Select(t => new TaskDto(
            t.Id,
            t.ProjectId,
            t.Title,
            t.Description,
            t.Status,
            t.DueDate,
            t.CreatedAt,
            t.UpdatedAt)).ToList();

        return new PagedResult<TaskDto>(dtos, page, pageSize, total);
    }

    public async Task<TaskDto> CreateTaskAsync(Guid ownerId, Guid projectId, CreateTaskRequest req, CancellationToken ct)
    {
        var project = await _projects.GetProjectAsync(projectId, ownerId, ct) ?? throw new InvalidOperationException("Project not found.");

        var title = req.Title?.Trim();
        if (string.IsNullOrWhiteSpace(title)) throw new InvalidOperationException("Task title is required.");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            ProjectId = project.Id,
            Title = title,
            Description = req.Description?.Trim(),
            DueDate = req.DueDate,
            CreatedAt = _clock.UtcNow,
            UpdatedAt = _clock.UtcNow
        };

        _projects.AddTask(task);
        project.UpdatedAt = _clock.UtcNow;

        await _uow.SaveChangesAsync(ct);

        return new TaskDto(task.Id, task.ProjectId, task.Title, task.Description, task.Status, task.DueDate, task.CreatedAt, task.UpdatedAt);
    }
}
