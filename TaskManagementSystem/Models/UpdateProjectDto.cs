using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class UpdateProjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<string> AssignedUserIds { get; set; } = new List<string>();
    }
}
