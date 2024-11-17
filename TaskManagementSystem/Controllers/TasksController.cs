using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public TasksController(ITaskService taskService, IUserService userService)
        {
            _taskService = taskService;
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isAdmin = await _userService.IsAdminAsync(User.Identity.Name);
            var isProjectOwner = await _userService.IsProjectOwnerAsync(createTaskDto.ProjectId, User.Identity.Name);

            if (!isAdmin && !isProjectOwner)
                return Forbid();

            var task = await _taskService.CreateTaskAsync(createTaskDto.ProjectId, createTaskDto, User.Identity.Name);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        [HttpGet("gettask/{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, User.Identity.Name);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isAdmin = await _userService.IsAdminAsync(User.Identity.Name);
            var isProjectOwner = await _userService.IsProjectOwnerAsync(id, User.Identity.Name);
            var task = await _taskService.GetTaskByIdAsync(id, User.Identity.Name);

            if (task == null)
                return NotFound();

            if (!isAdmin && !(isProjectOwner || task.AssignedUserIds.Contains(User.Identity.Name)))
                return Forbid(); 

            await _taskService.UpdateTaskAsync(id, updateTaskDto, User.Identity.Name);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var isAdmin = await _userService.IsAdminAsync(User.Identity.Name);
            var isProjectOwner = await _userService.IsProjectOwnerByTaskIdAsync(id, User.Identity.Name);

            if (!isAdmin && !isProjectOwner)
                return Forbid();

            var result = await _taskService.DeleteTaskAsync(id, User.Identity.Name);
            if (!result)
                return Forbid();

            return NoContent();
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var isAdmin = await _userService.IsAdminAsync(User.Identity.Name);
            var isProjectOwner = await _userService.IsProjectOwnerAsync(projectId, User.Identity.Name);
            var isProjectMember = await _userService.IsProjectMemberAsync(projectId, User.Identity.Name);

            if (!isAdmin && !isProjectOwner && !isProjectMember)
            {
                return Forbid();
            }

            var tasks = await _taskService.GetTasksByProjectIdAsync(projectId, User.Identity.Name);
            return Ok(tasks);
        }

        [HttpPost("add-user/{taskId}")]
        public async Task<IActionResult> AddMembersToTask(int taskId, [FromBody] List<string> memberEmails)
        {
            var userName = User.Identity.Name;

            try
            {
                bool result = await _taskService.AddMembersToTaskAsync(taskId, memberEmails, userName);
                if (result)
                {
                    return Ok(new { message = "Members added successfully to the task." });
                }

                return BadRequest("Failed to add members to the task.");
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("remove-user/{taskId}")]
        public async Task<IActionResult> RemoveMembersFromTask(int taskId, [FromBody] List<string> memberEmails)
        {
            var userName = User.Identity.Name;

            try
            {
                bool result = await _taskService.RemoveMembersFromTaskAsync(taskId, memberEmails, userName);
                if (result)
                {
                    return Ok(new { message = "Members removed successfully from the task." });
                }

                return BadRequest("Failed to remove members from the task.");
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpGet("user-tasks")]
        public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetUserTasks()
        {
            var userName = User.Identity.Name; 
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized("User is not authenticated.");
            }

            try
            {
                var tasks = await _taskService.GetUserTasksAsync(userName);
                return Ok(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
    }
}
