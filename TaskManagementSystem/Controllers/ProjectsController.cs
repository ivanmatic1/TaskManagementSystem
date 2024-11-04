using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto projectDto)
    {
        var userName = User.Identity.Name;
        if (userName == null) return Unauthorized();

        try
        {
            var project = await _projectService.CreateProjectAsync(projectDto, userName);
            return Ok(project);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User not authorized.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the project: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserProjects()
    {
        var userName = User.Identity.Name;
        if (userName == null) return Unauthorized();

        try
        {
            var projects = await _projectService.GetUserProjectsAsync(userName);
            return Ok(projects);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while fetching projects: {ex.Message}");
        }
    }

    [HttpPut("{projectId}")]
    public async Task<IActionResult> UpdateProject(int projectId, [FromBody] UpdateProjectDto projectDto)
    {
        var userName = User.Identity.Name;

        if (userName == null)
        {
            return Unauthorized();
        }

        try
        {
            var updatedProject = await _projectService.UpdateProjectAsync(projectId, projectDto, userName);
            return Ok(updatedProject);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the project: {ex.Message}");
        }
    }

    [HttpDelete("{projectId}")]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        var userName = User.Identity.Name;
        if (userName == null) return Unauthorized();

        try
        {
            var result = await _projectService.DeleteProjectAsync(projectId, userName);
            return result ? Ok() : Forbid();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the project: {ex.Message}");
        }
    }
}
