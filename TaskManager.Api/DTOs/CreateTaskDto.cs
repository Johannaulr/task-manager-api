using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TaskManager.Api.Models;

namespace TaskManager.Api.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title {get; set;} = string.Empty;

        [Required]
        public TaskPriority Priority {get; set;}

        public DateTime? DueDate {get; set;}
    }
}