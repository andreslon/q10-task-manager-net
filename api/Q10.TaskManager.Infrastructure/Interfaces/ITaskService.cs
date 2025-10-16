using Q10.TaskManager.Infrastructure.Entities;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface ITaskService
    {
        // Define methods for task management, e.g., CreateTask, GetTask, UpdateTask, DeleteTask
        Task<IEnumerable<TaskItem>> GetAllTasks();
        Task<TaskItem> GetTaskById(string id);
        Task<TaskItem> CreateTask(TaskItem task);
        Task<TaskItem> UpdateTask(string id, TaskItem task);
        Task<bool> DeleteTask(string id);
    }
}
