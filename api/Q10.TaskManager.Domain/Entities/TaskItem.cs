using System.ComponentModel.DataAnnotations;

namespace Q10.TaskManager.Domain.Entities
{
    public class TaskItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [MaxLength(400)]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime Created { get; set; } = DateTime.UtcNow;
        
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty", nameof(title));
            
            if (title.Length > 400)
                throw new ArgumentException("Title cannot exceed 400 characters", nameof(title));
            
            Title = title;
            Updated = DateTime.UtcNow;
        }

        public void UpdateDescription(string description)
        {
            if (description != null && description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters", nameof(description));
            
            Description = description ?? string.Empty;
            Updated = DateTime.UtcNow;
        }
    }
}


