using Q10.TaskManager.Application.DTOs.Responses;

namespace Q10.TaskManager.Application.Interfaces
{
    public interface ITaskBulkQueryService
    {
        Task<BulkTaskItemResponse?> GetTaskByIdAsync(string taskId);
    }
}

