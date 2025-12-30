namespace TaskManager.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public DateTime? dueDate {get; set;}
        public bool isCompleted {get; set;}
        
    }

    public enum Priority
    {
        Low,
        Medium,
        High
    }
}