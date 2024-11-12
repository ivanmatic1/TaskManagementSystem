using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [ForeignKey("Owner")]
        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        public ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
    }
}
