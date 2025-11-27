using System.ComponentModel.DataAnnotations;

namespace Q10.TaskManager.Application.DTOs.Requests
{
    public class TaskBulkRequest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

