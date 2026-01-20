using System;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;

namespace TaskManager.Api.Tests.Helpers;

public class CreateTaskDtoFactory
{
    public static CreateTaskDto Create
    (
        string title = "Default Task DTO",
        DateTime? dueDate = null,
        TaskPriority priority = TaskPriority.Low
    )
    {
        return new CreateTaskDto
        {
            Title = title,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(7),
            Priority = priority
        };
    }
}
