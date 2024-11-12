using System;
using System.Collections.Generic;
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

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
        public List<string> MemberEmails { get; set; } = new List<string>();
    }
}
