using TaskManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ProjectTaskDto> CreateTaskAsync(int projectId, CreateTaskDto createTaskDto, string userName)
        {
            var project = await _context.Projects.Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.Owner.UserName == userName);
            if (project == null)
                throw new UnauthorizedAccessException("It is not allowed to create tasks for this project.");

            var task = new ProjectTask
            {
                Name = createTaskDto.Name,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                ProjectId = projectId,
                CreatedAt = DateTime.Now 
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            if (createTaskDto.AssignedUserIds != null && createTaskDto.AssignedUserIds.Any())
            {
                var users = await _userManager.Users
                    .Where(u => createTaskDto.AssignedUserIds.Contains(u.UserName))
                    .ToListAsync();


                task.AssignedUsers = users;

                await _context.SaveChangesAsync();
            }

            return new ProjectTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                AssignedUserIds = task.AssignedUsers.Select(u => u.UserName).ToList()
            };
        }


        public async Task<ProjectTaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, string userName)
        {

            var task = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Owner)
                .Include(t => t.AssignedUsers)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Owner.UserName == userName);

            if (task == null)
                throw new UnauthorizedAccessException("Not allowed to update this task.");

            task.Name = updateTaskDto.Name;
            task.Description = updateTaskDto.Description;
            task.IsCompleted = updateTaskDto.IsCompleted;
            task.DueDate = updateTaskDto.DueDate;

            var assignedUsers = await _context.Users
                .Where(u => updateTaskDto.AssignedUserIds.Contains(u.UserName))
                .ToListAsync();

            task.AssignedUsers = assignedUsers;

            await _context.SaveChangesAsync();

            return new ProjectTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                AssignedUserIds = task.AssignedUsers.Select(u => u.Id).ToList()
            };
        }

        public async Task<bool> DeleteTaskAsync(int id, string userName)
        {
            var task = await _context.Tasks.Include(t => t.Project).ThenInclude(p => p.Owner)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Owner.UserName == userName);
            if (task == null)
                throw new UnauthorizedAccessException("Not allowed to delete this task.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProjectTaskDto> GetTaskByIdAsync(int id, string userName)
        {
            var task = await _context.Tasks.Include(t => t.Project).ThenInclude(p => p.Owner)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project.Owner.UserName == userName);
            if (task == null)
                throw new KeyNotFoundException("The task was not found or the user does not have access.");

            return new ProjectTaskDto { Id = task.Id, Name = task.Name, Description = task.Description };
        }

        public async Task<IEnumerable<ProjectTaskDto>> GetTasksByProjectIdAsync(int projectId, string userName)
        {
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted 
                })
                .ToListAsync();

            return tasks; 
        }
    }
}
