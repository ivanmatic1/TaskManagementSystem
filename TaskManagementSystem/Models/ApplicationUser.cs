using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {   
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    }
}
