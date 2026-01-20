using System;
using TaskManager.Api.Models;

namespace TaskManager.Api.Tests.Helpers;

public class TaskItemFactory
{
    public static TaskItem Create
    (
        string title = "Default Task",
        DateTime? dueDate = null,
        int priority = 0,
        int progress = 0
    )
    {
        return new TaskItem
        {
            Title = title,
            DueDate = dueDate ?? DateTime.UtcNow.AddDays(7),
            Priority = priority,
            Progress = progress
        };
    }
}
