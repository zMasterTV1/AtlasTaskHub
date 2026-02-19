using Atlas.Application.Contracts.Projects;
using Atlas.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects")]
public sealed class ProjectsController : ControllerBase
{
    private readonly ProjectsService _projects;

    public ProjectsController(ProjectsService projects) => _projects = projects;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var userId = HttpContext.GetUserId();
        return Ok(await _projects.ListProjectsAsync(userId, page, pageSize, ct));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var created = await _projects.CreateProjectAsync(userId, req, ct);
        return CreatedAtAction(nameof(List), new { id = created.Id }, created);
    }

    [HttpGet("{projectId:guid}/tasks")]
    public async Task<IActionResult> ListTasks(
        [FromRoute] Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? q = null,
        CancellationToken ct = default)
    {
        var userId = HttpContext.GetUserId();
        return Ok(await _projects.ListTasksAsync(userId, projectId, page, pageSize, q, ct));
    }

    [HttpPost("{projectId:guid}/tasks")]
    public async Task<IActionResult> CreateTask([FromRoute] Guid projectId, [FromBody] CreateTaskRequest req, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        var created = await _projects.CreateTaskAsync(userId, projectId, req, ct);
        return Ok(created);
    }
}
