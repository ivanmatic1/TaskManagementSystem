using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services
{
    public interface IUserService
    {
        Task<bool> IsAdminAsync(string userEmail);
        Task<bool> IsProjectOwnerAsync(int projectId, string userEmail);
        Task<bool> IsProjectMemberAsync(int projectId, string userName);
        Task<bool> AssignAdminRoleAsync(string userEmail);
        Task<bool> RemoveAdminRoleAsync(string userEmail);
        Task<bool> DeleteUserAsync(string userEmail);
        Task<IEnumerable<UserListDto>> GetUsersAsync();
    }
}
