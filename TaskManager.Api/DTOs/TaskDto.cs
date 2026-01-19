using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs
{
    public record class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TaskProgress Progress { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime? DueDate { get; set;}
        public List<TagDto> Tags { get; set; } = new();
    }
}