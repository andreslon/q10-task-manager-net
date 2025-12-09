using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Q10.TaskManager.Domain.Entities
{
    public class TaskItem
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [MaxLength(400)]
        public string Title { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Updated { get; set; } = DateTime.UtcNow;

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                throw new ArgumentException("Title cannot be null or empty");
            if (title.Length > 400)
                throw new ArgumentException("Title cannot exceed 400 characters");
            Title = title;
            Updated = DateTime.UtcNow;
        }
        public void UpdateDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description cannot be null or empty");
            if (description != null && description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters");
            Description = description;
            Updated = DateTime.UtcNow;
        }
    }
}

