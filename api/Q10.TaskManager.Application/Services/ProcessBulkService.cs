using Q10.TaskManager.Application.DTOs.Commands;
using Q10.TaskManager.Application.DTOs.Responses;
using Q10.TaskManager.Application.Interfaces;
using Q10.TaskManager.Domain.Entities;
using Q10.TaskManager.Domain.Interfaces;

namespace Q10.TaskManager.Application.Services
{
    public class ProcessBulkService : IProcessBulkService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMessageQueueRepository _messageQueueRepository;

        public ProcessBulkService(ITaskRepository taskRepository, IMessageQueueRepository messageQueueRepository)
        {
            _taskRepository = taskRepository;
            _messageQueueRepository = messageQueueRepository;
        }

        public async Task StartConsumingAsync()
        {
            await _messageQueueRepository.StartConsumingAsync<TaskBulkCommand>(
                "task-bulk-queue", 
                ProcessBulkCommand);
        }

        public async Task ProcessBulkCommand(TaskBulkCommand command)
        {
            var results = new List<BulkTaskItemResponse>();

            foreach (var taskRequest in command.Tasks)
            {
                try
                {
                    var taskItem = new TaskItem
                    {
                        Id = taskRequest.Id,
                        Title = taskRequest.Title,
                        Description = taskRequest.Description
                    };

                    await _taskRepository.CreateTaskAsync(taskItem);

                    results.Add(new BulkTaskItemResponse
                    {
                        TaskId = taskItem.Id,
                        Title = taskItem.Title,
                        IsSuccess = true
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new BulkTaskItemResponse
                    {
                        TaskId = string.Empty,
                        Title = taskRequest.Title,
                        IsSuccess = false,
                        ErrorMessage = ex.Message
                    });
                }
            } 
        }
    }
}

