using Q10.TaskManager.Application.DTOs.Responses;
using Q10.TaskManager.Application.Interfaces;
using Q10.TaskManager.Domain.Interfaces;

namespace Q10.TaskManager.Application.Services
{
    public class TaskBulkQueryService : ITaskBulkQueryService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskBulkQueryService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<BulkTaskItemResponse?> GetTaskByIdAsync(string taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                return null;
            }

            return new BulkTaskItemResponse
            {
                TaskId = task.Id,
                Title = task.Title,
                IsSuccess = true
            };
        }
    }
}

