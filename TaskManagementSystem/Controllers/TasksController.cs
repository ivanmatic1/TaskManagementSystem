using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers
{
    [Authorize]  // Ensure user is authenticated with JWT
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var task = await _taskService.CreateTaskAsync(createTaskDto.ProjectId, createTaskDto, User.Identity.Name);
            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, User.Identity.Name);

            if (task == null)
                return NotFound();

            return Ok(task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _taskService.UpdateTaskAsync(id, updateTaskDto, User.Identity.Name);
            if (result == null) // Check if result is null
                return Forbid();

            return NoContent();
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id, User.Identity.Name);
            if (!result)
                return Forbid();

            return NoContent();
        }

        // GET: api/tasks/project/{projectId}
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(int projectId)
        {
            var tasks = await _taskService.GetTasksByProjectIdAsync(projectId, User.Identity.Name);
            return Ok(tasks);
        }
    }
}
