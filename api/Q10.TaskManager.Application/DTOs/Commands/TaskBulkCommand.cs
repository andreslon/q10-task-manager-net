using Q10.TaskManager.Application.DTOs.Requests;

namespace Q10.TaskManager.Application.DTOs.Commands
{
    public class TaskBulkCommand
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public List<TaskBulkRequest> Tasks { get; set; } = new List<TaskBulkRequest>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
