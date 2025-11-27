using Q10.TaskManager.Application.DTOs.Commands;
using Q10.TaskManager.Application.DTOs.Requests;
using Q10.TaskManager.Application.Interfaces;

namespace Q10.TaskManager.Application.Services
{
    public class TaskBulkCommandService : ITaskBulkCommandService
    {
        private readonly IMessageQueueRepository _messageQueueRepository;

        public TaskBulkCommandService(IMessageQueueRepository messageQueueRepository)
        {
            _messageQueueRepository = messageQueueRepository;
        }

        public async Task<List<string>> ProcessBulkTasksAsync(List<TaskBulkRequest> tasks)
        {
            var command = new TaskBulkCommand
            {
                Tasks = tasks
            };

            await _messageQueueRepository.PublishAsync(command, "task-bulk-queue");

            var taskIds = tasks.Select(t => t.Id).ToList();
            return taskIds;
        }
    }
}

