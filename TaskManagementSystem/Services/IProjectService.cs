using TaskManagementSystem.Models;


namespace TaskManagementSystem.Services
{
    public interface IProjectService
    {
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, string userName);
        Task<ProjectDto> UpdateProjectAsync(int id, UpdateProjectDto updateProjectDto, string userName);
        Task<bool> DeleteProjectAsync(int id, string userName);
        Task<ProjectDto> GetProjectByIdAsync(int id, string userName);
        Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(string userName);
    }


}
