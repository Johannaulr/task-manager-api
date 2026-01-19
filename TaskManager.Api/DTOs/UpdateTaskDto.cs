using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs
{
    public record class UpdateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title {get; set;} = string.Empty;

        [Required]
        public TaskPriority Priority {get; set;}

        public DateTime? DueDate {get; set;}
        public TaskProgress Progress {get; set;}
    }
}