﻿using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public ICollection<string> AssignedUserIds { get; set; } = new List<string>();
    }
}
