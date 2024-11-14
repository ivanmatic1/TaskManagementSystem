using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;


namespace TaskManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            string userName = User.Identity.Name;

            try
            {
                var project = await _projectService.CreateProjectAsync(createProjectDto, userName);
                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            string userName = User.Identity.Name;

            try
            {
                var project = await _projectService.UpdateProjectAsync(id, updateProjectDto, userName);
                return Ok(project);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProject(int id)
        {
            string userName = User.Identity.Name;

            try
            {
                await _projectService.DeleteProjectAsync(id, userName);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getproject/{id}")]
        [Authorize]
        public async Task<IActionResult> GetProjectById(int id)
        {
            string userName = User.Identity.Name;

            try
            {
                var project = await _projectService.GetProjectByIdAsync(id, userName);
                return Ok(project);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getprojects")]
        [Authorize]
        public async Task<IActionResult> GetUserProjects()
        {
            string userName = User.Identity.Name;

            try
            {
                var projects = await _projectService.GetUserProjectsAsync(userName);
                return Ok(projects);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("addmember/{projectId}")]
        [Authorize]
        public async Task<IActionResult> AddMembersToProject(int projectId, [FromBody] List<string> memberEmails)
        {
            var userName = User.Identity.Name;
            var result = await _projectService.AddMembersToProjectAsync(projectId, memberEmails, userName);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to add members.");
        }

        [HttpDelete("removemember/{projectId}")]
        [Authorize]
        public async Task<IActionResult> RemoveMembersFromProject(int projectId, [FromBody] List<string> memberEmails)
        {
            var userName = User.Identity.Name;
            var result = await _projectService.RemoveMembersFromProjectAsync(projectId, memberEmails, userName);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to remove members.");
        }

        [HttpGet("{projectId}/users")]
        [Authorize]
        public async Task<ActionResult<List<UserListDto>>> GetAllUsersInProject(int projectId)
        {
            var userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var users = await _projectService.GetAllUsersInProjectAsync(projectId, userName);
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


    }
}
