using Q10.TaskManager.Infrastructure.DTOs;
using Q10.TaskManager.Infrastructure.Interfaces;

namespace Q10.TaskManager.Infrastructure.Services
{
    public class ProcessBulkService : IProcessBulkService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IRabbitMQRepository _rabbitMQRepository;

        public ProcessBulkService(ITaskRepository taskRepository, IRabbitMQRepository rabbitMQRepository)
        {
            _taskRepository = taskRepository;
            _rabbitMQRepository = rabbitMQRepository;
        }

        public async Task StartConsumingAsync()
        {
            await _rabbitMQRepository.StartConsumingAsync<TaskBulkCommand>(
                "task-bulk-queue", 
                ProcessBulkCommand);
        }

        public async Task ProcessBulkCommand(TaskBulkCommand command)
        {
            var results = new List<TaskBulkResponse>();

            foreach (var taskRequest in command.Tasks)
            {
                try
                {
                    var taskItem = new Entities.TaskItem
                    {
                        Id = taskRequest.Id, // Usar el ID que ya viene en el request
                        Title = taskRequest.Title,
                        Description = taskRequest.Description
                    };

                    await _taskRepository.CreateTaskAsync(taskItem);

                    results.Add(new TaskBulkResponse
                    {
                        TaskId = taskItem.Id,
                        Title = taskItem.Title,
                        IsSuccess = true
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new TaskBulkResponse
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
