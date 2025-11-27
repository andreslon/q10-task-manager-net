namespace Q10.TaskManager.Application.DTOs.Responses
{
    public class BulkProcessResponse
    {
        public string CommandId { get; set; } = string.Empty;
        public List<BulkTaskItemResponse> Results { get; set; } = new List<BulkTaskItemResponse>();
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}


