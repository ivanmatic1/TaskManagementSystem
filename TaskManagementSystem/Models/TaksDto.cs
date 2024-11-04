namespace TaskManagementSystem.Models
{
    public class TaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }

        public List<string> AssignedUserIds { get; set; }
    }
}
