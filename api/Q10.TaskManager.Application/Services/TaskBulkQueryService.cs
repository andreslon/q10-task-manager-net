using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Application.Services
{
    public class TaskBulkQueryService : ITaskBulkQueryService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskBulkQueryService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskBulkResponse> GetTaskByIdAsync(string taskId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(taskId);
            
            if (task == null)
            {
                return null;
            }

            return new TaskBulkResponse
            {
                TaskId = task.Id,
                Title = task.Title,
                IsSuccess = true
            };
        }
    }
}
