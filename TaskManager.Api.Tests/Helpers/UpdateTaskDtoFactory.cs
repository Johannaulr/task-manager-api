using System;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;

namespace TaskManager.Api.Tests.Helpers;

public class UpdateTaskDtoFactory
{
    public static UpdateTaskDto Create
    (
        string title = "Updated Task DTO",
        DateTime? dueDate = null,
        TaskPriority priority = TaskPriority.Medium
    )
    {
        return new UpdateTaskDto
        {
            Title = title,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(10),
            Priority = priority
        };
    }
}
