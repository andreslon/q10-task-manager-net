using Q10.TaskManager.Domain.Entities;

namespace Q10.TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasks();
        Task<TaskItem?> GetTaskById(string id);
        Task<TaskItem> CreateTask(TaskItem task);
        Task<TaskItem> UpdateTask(string id, TaskItem task);
        Task<bool> DeleteTask(string id);
    }
}

