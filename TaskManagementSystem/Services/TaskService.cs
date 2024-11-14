using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;


public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public TaskService(AppDbContext context, UserManager<ApplicationUser> userManager, IUserService userService)
    {
        _context = context;
        _userManager = userManager;
        _userService = userService;
    }

    public async Task<ProjectTaskDto> CreateTaskAsync(int projectId, CreateTaskDto createTaskDto, string userName)
    {
        var project = await _context.Projects
            .Include(p => p.Owner)
            .FirstOrDefaultAsync(p => p.Id == projectId && p.Owner.UserName == userName);

        if (project == null)
        {
            if (!await _userService.IsAdminAsync(userName))
                throw new UnauthorizedAccessException("It is not allowed to create tasks for this project.");
        }

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
            var assignedUsers = new List<ApplicationUser>();
            foreach (var email in createTaskDto.AssignedUserIds)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    throw new KeyNotFoundException($"User with email {email} not found.");
                assignedUsers.Add(user);
            }

            task.AssignedUsers = assignedUsers;
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
        var isAdmin = await _userService.IsAdminAsync(userName);

        var task = await _context.Tasks
            .Include(t => t.Project)
                .ThenInclude(p => p.Owner)
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == id &&
                                       (t.Project.Owner.UserName == userName || 
                                        t.AssignedUsers.Any(u => u.UserName == userName) ||
                                        isAdmin)); 

        if (task == null)
            throw new UnauthorizedAccessException("Not allowed to update this task.");

        if (!isAdmin && !task.AssignedUsers.Any(u => u.UserName == userName))
            throw new UnauthorizedAccessException("You do not have permission to update this task.");

        task.Name = updateTaskDto.Name;
        task.Description = updateTaskDto.Description;
        task.IsCompleted = updateTaskDto.IsCompleted;
        task.DueDate = updateTaskDto.DueDate;

        if (updateTaskDto.AssignedUserIds != null && updateTaskDto.AssignedUserIds.Any())
        {
            var updatedAssignedUsers = new List<ApplicationUser>();
            foreach (var email in updateTaskDto.AssignedUserIds)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    throw new KeyNotFoundException($"User with email {email} not found.");
                updatedAssignedUsers.Add(user);
            }

            task.AssignedUsers = updatedAssignedUsers;
        }

        await _context.SaveChangesAsync();

        return new ProjectTaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            DueDate = task.DueDate,
            IsCompleted = task.IsCompleted,
            AssignedUserIds = task.AssignedUsers.Select(u => u.UserName).ToList()
        };
    }

    public async Task<bool> DeleteTaskAsync(int id, string userName)
    {
        var isAdmin = await _userService.IsAdminAsync(userName);

        var task = await _context.Tasks
            .Include(t => t.Project)
                .ThenInclude(p => p.Owner)
            .FirstOrDefaultAsync(t => t.Id == id &&
                                       (t.Project.Owner.UserName == userName ||
                                        t.AssignedUsers.Any(u => u.UserName == userName) ||
                                        isAdmin));


        if (task == null)
            throw new UnauthorizedAccessException("Not allowed to delete this task.");

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProjectTaskDto> GetTaskByIdAsync(int id, string userName)
    {
        var isAdmin = await _userService.IsAdminAsync(userName);

        var task = await _context.Tasks
            .Include(t => t.Project)
                .ThenInclude(p => p.Owner)
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == id &&
                                       (t.Project.Owner.UserName == userName ||
                                        t.AssignedUsers.Any(u => u.UserName == userName) ||
                                        isAdmin));

        if (task == null)
        {
            throw new UnauthorizedAccessException("The task was not found or the user does not have access.");
        }

        return new ProjectTaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            AssignedUserIds = task.AssignedUsers.Select(u => u.UserName).ToList()
        };
    }

    public async Task<IEnumerable<ProjectTaskDto>> GetTasksByProjectIdAsync(int projectId, string userName)
    {
        var isAdmin = await _userService.IsAdminAsync(userName);

        var project = await _context.Projects
            .Include(p => p.Owner)
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == projectId &&
                                      (p.Owner.UserName == userName ||
                                       p.Members.Any(m => m.UserName == userName) ||
                                       isAdmin));

        if (project == null)
            throw new UnauthorizedAccessException("User does not have access to this project.");

        if (isAdmin || project.Owner.UserName == userName)
        {
            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    DueDate = t.DueDate,
                    AssignedUserIds = t.AssignedUsers.Select(u => u.UserName).ToList()
                })
                .ToListAsync();

            return tasks;
        }

        var tasksForUser = await _context.Tasks
            .Where(t => t.ProjectId == projectId && t.AssignedUsers.Any(u => u.UserName == userName))
            .Select(t => new ProjectTaskDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                DueDate = t.DueDate,
                AssignedUserIds = t.AssignedUsers.Select(u => u.UserName).ToList()
            })
            .ToListAsync();

        return tasksForUser;
    }

    public async Task<IEnumerable<ProjectTaskDto>> GetUserTasksAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        bool isAdmin = await _userService.IsAdminAsync(userName);

        IEnumerable<ProjectTask> tasks;

        if (isAdmin)
        {
            tasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .ToListAsync();
        }
        else
        {
            tasks = await _context.Tasks
                .Include(t => t.Project)
                .Include(t => t.AssignedUsers)
                .Where(t => t.Project.OwnerId == user.Id || t.AssignedUsers.Any(u => u.Id == user.Id))
                .ToListAsync();
        }

        return tasks.Select(t => new ProjectTaskDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            IsCompleted = t.IsCompleted,
            DueDate = t.DueDate,
            ProjectId = t.ProjectId,
            AssignedUserIds = t.AssignedUsers.Select(u => u.UserName).ToList()
        });
    }
    public async Task<bool> AddMembersToTaskAsync(int taskId, List<string> memberEmails, string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        bool isAdmin = await _userService.IsAdminAsync(userName);
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new UnauthorizedAccessException("Task not found.");
        }

        if (!isAdmin && task.Project.OwnerId != user.Id)
        {
            throw new UnauthorizedAccessException("Only admins or project owners can add members to this task.");
        }

        var membersToAdd = new List<ApplicationUser>();
        foreach (var email in memberEmails)
        {
            var member = await _userManager.FindByEmailAsync(email);
            if (member == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }
            membersToAdd.Add(member);
        }

        foreach (var member in membersToAdd)
        {
            if (!task.AssignedUsers.Contains(member))
            {
                task.AssignedUsers.Add(member);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> RemoveMembersFromTaskAsync(int taskId, List<string> memberEmails, string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        bool isAdmin = await _userService.IsAdminAsync(userName);
        var task = await _context.Tasks
            .Include(t => t.AssignedUsers)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new UnauthorizedAccessException("Task not found.");
        }

        if (!isAdmin && task.Project.OwnerId != user.Id)
        {
            throw new UnauthorizedAccessException("Only admins or project owners can remove members from this task.");
        }

        foreach (var email in memberEmails)
        {
            var member = await _userManager.FindByEmailAsync(email);
            if (member == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }

            if (task.AssignedUsers.Contains(member))
            {
                task.AssignedUsers.Remove(member);
            }
            else
            {
                throw new KeyNotFoundException($"User with email {email} is not assigned to this task.");
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

}
