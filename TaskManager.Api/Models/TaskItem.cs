using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; } // 0 = Low, 1 = Medium, 2 = High, 3 = Critical
        public DateTime? DueDate {get; set;}
        public int Progress {get; set;} // 0 = NotStarted, 1 = InProgress, 2 = Completed
    }
}