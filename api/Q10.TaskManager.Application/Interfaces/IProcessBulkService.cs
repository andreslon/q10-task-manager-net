using Q10.TaskManager.Infrastructure.DTOs;

namespace Q10.TaskManager.Infrastructure.Interfaces
{
    public interface IProcessBulkService
    {
        Task ProcessBulkCommand(TaskBulkCommand command);
        Task StartConsumingAsync();
    }
}

