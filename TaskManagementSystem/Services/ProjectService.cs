using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;

        public ProjectService(AppDbContext context, UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
        }
        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            var members = new List<ApplicationUser> { user };

            if (createProjectDto.MemberEmails != null && createProjectDto.MemberEmails.Any())
            {
                foreach (var email in createProjectDto.MemberEmails)
                {
                    var member = await _userManager.FindByEmailAsync(email);
                    if (member == null)
                        throw new KeyNotFoundException($"User with email {email} not found.");

                    members.Add(member);
                }
            }

            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                StartDate = createProjectDto.StartDate,
                EndDate = createProjectDto.EndDate,
                Owner = user,
                Members = members
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                OwnerName = project.Owner.UserName,
                MemberIds = project.Members.Select(m => m.Id).ToList()
            };
        }
        public async Task<ProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto updateProjectDto, string userName)
        {
            var project = await _context.Projects.Include(p => p.Members)
                                                 .FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
                throw new KeyNotFoundException("Project not found.");

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            bool isAdmin = await _userService.IsAdminAsync(userName);
            if (!isAdmin && user.Id != project.OwnerId)
            {
                throw new UnauthorizedAccessException("Only admins or the project owner can update this project.");
            }

            project.Name = updateProjectDto.Name;
            project.Description = updateProjectDto.Description;
            project.StartDate = updateProjectDto.StartDate;
            project.EndDate = updateProjectDto.EndDate;

            var updatedMembers = new List<ApplicationUser>();
            foreach (var email in updateProjectDto.MemberEmails)
            {
                var member = await _userManager.FindByEmailAsync(email);
                if (member == null)
                    throw new KeyNotFoundException($"User with email {email} not found.");
                updatedMembers.Add(member);
            }

            project.Members = updatedMembers;

            _context.Projects.Update(project);
            await _context.SaveChangesAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                OwnerName = project.Owner.UserName,
                MemberIds = project.Members.Select(m => m.Id).ToList()
            };
        }

        public async Task<bool> DeleteProjectAsync(int projectId, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            bool isAdmin = await _userService.IsAdminAsync(userName);
            if (!isAdmin)
            {
                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.Id == projectId && p.OwnerId == user.Id);

                if (project == null)
                {
                    throw new UnauthorizedAccessException("Project not found or user is not authorized.");
                }
            }

            var projectToDelete = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (projectToDelete == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            _context.Projects.Remove(projectToDelete);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<ProjectDto> GetProjectByIdAsync(int projectId, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var isAdmin = await _userService.IsAdminAsync(userName);

            var project = await _context.Projects
                .Include(p => p.Members)
                .Include(p => p.Owner) 
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            if (project.Owner == null)
            {
                throw new KeyNotFoundException("Project owner not found.");
            }

            if (project.OwnerId != user.Id && !project.Members.Any(m => m.Id == user.Id) && !isAdmin)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this project.");
            }

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                OwnerName = project.Owner.UserName,
                MemberIds = project.Members.Select(m => m.Id).ToList()
            };
        }
        public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            bool isAdmin = await _userService.IsAdminAsync(userName);

            IEnumerable<Project> projects;

            if (isAdmin)
            {
                projects = await _context.Projects
                    .Include(p => p.Members)
                    .ToListAsync();
            }
            else
            {
                projects = await _context.Projects
                    .Where(p => p.OwnerId == user.Id || p.Members.Any(m => m.Id == user.Id))
                    .Include(p => p.Members)
                    .ToListAsync();
            }

            return projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                OwnerId = p.OwnerId,
                OwnerName = p.Owner != null ? p.Owner.UserName : "No Owner", // Null check for Owner
                MemberIds = p.Members.Select(m => m.Id).ToList()
            });
        }

        public async Task<bool> AddMembersToProjectAsync(int projectId, List<string> memberEmails, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            bool isAdmin = await _userService.IsAdminAsync(userName);
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                throw new UnauthorizedAccessException("Project not found.");
            }

            if (!isAdmin && project.OwnerId != user.Id)
            {
                throw new UnauthorizedAccessException("Only admins or project owners can add members.");
            }

            var members = new List<ApplicationUser>();
            foreach (var email in memberEmails)
            {
                var member = await _userManager.FindByEmailAsync(email);
                if (member == null)
                {
                    throw new KeyNotFoundException($"User with email {email} not found.");
                }
                members.Add(member);
            }

            foreach (var member in members)
            {
                project.Members.Add(member);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RemoveMembersFromProjectAsync(int projectId, List<string> memberEmails, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            bool isAdmin = await _userService.IsAdminAsync(userName);
            var project = await _context.Projects
                .Include(p => p.Members)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                throw new UnauthorizedAccessException("Project not found.");
            }

            if (!isAdmin && project.OwnerId != user.Id)
            {
                throw new UnauthorizedAccessException("Only admins or project owners can remove members.");
            }

            foreach (var email in memberEmails)
            {
                var member = await _userManager.FindByEmailAsync(email);
                if (member == null)
                {
                    throw new KeyNotFoundException($"User with email {email} not found.");
                }

                project.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<UserListDto>> GetAllUsersInProjectAsync(int projectId, string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }

            var isAdmin = await _userService.IsAdminAsync(userName);
            var project = await _context.Projects
                .Include(p => p.Members)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
            {
                throw new KeyNotFoundException("Project not found.");
            }

            if (project.OwnerId != user.Id && !project.Members.Any(m => m.Id == user.Id) && !isAdmin)
            {
                throw new UnauthorizedAccessException("You are not authorized to view the users in this project.");
            }

            var users = new List<ApplicationUser> { project.Owner };
            users.AddRange(project.Members);

            // Create a DTO list
            var userDtos = new List<UserListDto>();
            foreach (var u in users.Distinct())
            {
                var roles = await _userManager.GetRolesAsync(u);

                userDtos.Add(new UserListDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    Email = u.Email,
                    Roles = roles.ToList()
                });
            }

            return userDtos;
        }

    }
}
