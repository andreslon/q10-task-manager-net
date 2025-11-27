using Q10.TaskManager.Application.DTOs.Requests;

namespace Q10.TaskManager.Application.Interfaces
{
    public interface ITaskBulkCommandService
    {
        Task<List<string>> ProcessBulkTasksAsync(List<TaskBulkRequest> tasks);
    }
}

