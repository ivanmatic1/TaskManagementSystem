using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? DueDate { get; set; }

        public int ProjectId { get; set; }

        public ICollection<string> AssignedUserIds { get; set; } = new List<string>();
    }
}
