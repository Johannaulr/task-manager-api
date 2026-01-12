using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime? DueDate {get; set;}
        public int Progress {get; set;}
    }
}