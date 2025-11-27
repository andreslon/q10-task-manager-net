using Q10.TaskManager.Application.DTOs.Commands;

namespace Q10.TaskManager.Application.Interfaces
{
    public interface IProcessBulkService
    {
        Task ProcessBulkCommand(TaskBulkCommand command);
        Task StartConsumingAsync();
    }
}


