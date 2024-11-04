using TaskManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectService(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                StartDate = createProjectDto.StartDate,
                EndDate = createProjectDto.EndDate,
                Owner = user
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDto { Id = project.Id, Name = project.Name, Description = project.Description };
        }

        public async Task<ProjectDto> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto, string userName)
        {
            var project = await _context.Projects.Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id && p.Owner.UserName == userName);
            if (project == null)
                throw new UnauthorizedAccessException("Not allowed to update this project.");

            project.Name = updateProjectDto.Name;
            project.Description = updateProjectDto.Description;
            project.StartDate = updateProjectDto.StartDate;
            project.EndDate = updateProjectDto.EndDate;

            await _context.SaveChangesAsync();

            return new ProjectDto { Id = project.Id, Name = project.Name, Description = project.Description };
        }

        public async Task<bool> DeleteProjectAsync(int id, string userName)
        {
            var project = await _context.Projects.Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id && p.Owner.UserName == userName);
            if (project == null)
                throw new UnauthorizedAccessException("Not allowed to delete this project.");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProjectDto> GetProjectByIdAsync(int id, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id && (p.Owner.UserName == userName || isAdmin));

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found or user does not have access.");
            }

            return new ProjectDto { Id = project.Id, Name = project.Name, Description = project.Description };
        }

        public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var projects = await _context.Projects
                .Where(p => p.Owner.UserName == userName)
                .ToListAsync();

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            });
        }
    }
}
