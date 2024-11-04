using TaskManagementSystem.Models;


namespace TaskManagementSystem.Services
{
    public interface ITaskService
    {
        Task<ProjectTaskDto> CreateTaskAsync(int projectId, CreateTaskDto createTaskDto, string userName);
        Task<ProjectTaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, string userName);
        Task<bool> DeleteTaskAsync(int id, string userName);
        Task<ProjectTaskDto> GetTaskByIdAsync(int id, string userName);
        Task<IEnumerable<ProjectTaskDto>> GetTasksByProjectIdAsync(int projectId, string userName);
    }
}