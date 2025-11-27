namespace Q10.TaskManager.Application.DTOs.Responses
{
    public class BulkTaskItemResponse
    {
        public string TaskId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

