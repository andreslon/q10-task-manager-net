using System.ComponentModel.DataAnnotations;

namespace Q10.TaskManager.Infrastructure.DTOs
{
    public class TaskBulkResponse
    {
        public string TaskId { get; set; }
        public string Title { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}
