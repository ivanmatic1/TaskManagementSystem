using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async Task<bool> IsAdminAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<bool> IsProjectOwnerAsync(int projectId, string userName)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.Owner.UserName == userName);
            return project != null;
        }

        public async Task<bool> IsProjectMemberAsync(int projectId, string userName)
        {
            var project = await _context.Projects
                .Where(p => p.Id == projectId)
                .Include(p => p.Members) 
                .FirstOrDefaultAsync();

            return project?.Members.Any(m => m.UserName == userName) ?? false;
        }

        public async Task<bool> AssignAdminRoleAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return false;
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (isAdmin)
            {
                return false;
            }

            var isUser = await _userManager.IsInRoleAsync(user, "User");
            if (isUser)
            {
                var removeUserRoleResult = await _userManager.RemoveFromRoleAsync(user, "User");
                if (!removeUserRoleResult.Succeeded)
                {
                    return false;
                }
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            return result.Succeeded;
        }

        public async Task<bool> RemoveAdminRoleAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return false;
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin)
            {
                return false;
            }

            var removeAdminRoleResult = await _userManager.RemoveFromRoleAsync(user, "Admin");
            if (!removeAdminRoleResult.Succeeded)
            {
                return false;
            }
            var addUserRoleResult = await _userManager.AddToRoleAsync(user, "User");
            return addUserRoleResult.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        public async Task<IEnumerable<UserListDto>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<UserListDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userList.Add(new UserListDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }
            return userList;
        }
    }
}
