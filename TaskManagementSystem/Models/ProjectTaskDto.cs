using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class ProjectTaskDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DueDate { get; set; }

        public int ProjectId { get; set; }

        public ICollection<string> AssignedUserIds { get; set; } = new List<string>(); 
    }
}
