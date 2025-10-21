using Q10.TaskManager.Infrastructure.DTOs;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface ITaskBulkCommandService
    {
        Task<string> ProcessBulkTasksAsync(List<TaskBulkRequest> tasks);
    }
}
