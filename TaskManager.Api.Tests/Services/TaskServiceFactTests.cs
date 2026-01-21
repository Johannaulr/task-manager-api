using System;
using Xunit;
using TaskManager.Api.Services;
using TaskManager.Api.Data;
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.DTOs;
using TaskManager.Api.Services.TaskResults;
using TaskManager.Api.Tests.Helpers;

namespace TaskManager.Api.Tests.Services;

public class TaskServiceFactTests
{

    // CreateTaskAsync Tests

    [Fact]
    public async Task CreateTaskAsync_DuplicateTitle_ShouldReturnDuplicateTaskTitle()
    {

        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var existingTask = TaskItemFactory.Create
        (
            title: "Test Task"
        );

        context.TaskItems.Add(existingTask);
        await context.SaveChangesAsync();

        var newTaskDto = CreateTaskDtoFactory.Create
        (
            title: "Test Task" // Duplicate title
        );

        var result = await service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.DuplicateTaskTitle, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTaskAsync_DueDateInPast_ShouldReturnInvalidDueDate()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var newTaskDto = CreateTaskDtoFactory.Create
        (
            dueDate: DateTime.UtcNow.AddDays(-1) // Due date in the past
        );

        var result = await service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.DueDateInPast, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTaskAsync_InvalidInput_ShouldReturnInvalidInput()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var newTaskDto = CreateTaskDtoFactory.Create
        (
            title: " " // Invalid title (whitespace)
        );

        var result = await service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.ValidationFailed, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTaskAsync_ValidInput_ShouldReturnSuccess()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var newTaskDto = CreateTaskDtoFactory.Create
        (
            title: "New Valid Task", // Valid title
            dueDate: DateTime.UtcNow.AddDays(5) // Valid due date
        );

        var result = await service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.Success, result.Result);
        Assert.NotNull(result.Created);
        Assert.Equal(newTaskDto.Title, result.Created.Title);
        Assert.Equal(newTaskDto.Priority, result.Created.Priority);
        Assert.Equal(newTaskDto.DueDate, result.Created.DueDate);
    }

    // AddTagToTaskAsync Tests

    [Theory]
    [InlineData(999, 1)]
    [InlineData(1, 999)]
    public async Task AddTagToTaskAsync_TaskOrTagDoesNotExist_ShouldReturnNotFound(int taskId, int tagId)
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var result = await service.AddTagToTaskAsync(taskId, tagId);
        Assert.Equal(AddTagToTaskResult.TaskNotFound, result);
    }

    [Fact]
    public async Task AddTagToTaskAsync_TagAlreadyAdded_ShouldReturnTagAlreadyAdded()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var task = TaskItemFactory.Create
        (
            title: "Task with Tag"
        );

        var tag = new Tag
        {
            Name = "Important"
        };

        context.TaskItems.Add(task);
        context.Tags.Add(tag);

        await context.SaveChangesAsync();

        await service.AddTagToTaskAsync(task.Id, tag.Id);

        var result = await service.AddTagToTaskAsync(task.Id, tag.Id);
        Assert.Equal(AddTagToTaskResult.TagAlreadyAdded, result);
    }

    [Fact]
    public async Task AddTagToTaskAsync_ValidInput_ShouldReturnSuccess()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var task = TaskItemFactory.Create
        (
            title: "New Task with Tag"
        );

        var tag = new Tag
        {
            Name = "Urgent"
        };

        context.TaskItems.Add(task);
        context.Tags.Add(tag);

        await context.SaveChangesAsync();

        var result = await service.AddTagToTaskAsync(task.Id, tag.Id);
        Assert.Equal(AddTagToTaskResult.Success, result);
    }

    // UpdateTaskAsync Tests

    [Fact]
    public async Task UpdateTaskAsync_NonExistentTask_ShouldReturnNotFound()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: "Updated Title"
        );

        var result = await service.UpdateTaskAsync(999, updateTaskDto);
        Assert.Equal(UpdateTaskResult.NotFound, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_InvalidInput_ShouldReturnValidationFailed()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var existingTask = TaskItemFactory.Create
        (
            title: "Existing Task"
        );

        context.TaskItems.Add(existingTask);
        await context.SaveChangesAsync();

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: " " // Invalid title (whitespace)
        );

        var result = await service.UpdateTaskAsync(existingTask.Id, updateTaskDto);
        Assert.Equal(UpdateTaskResult.ValidationFailed, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_CompletedTask_ShouldReturnCannotUpdateCompletedTask()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var completedTask = TaskItemFactory.Create
        (
            title: "Completed Task",
            progress: 2 // Completed
        );

        context.TaskItems.Add(completedTask);
        await context.SaveChangesAsync();

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: "Attempt to Update Completed Task"
        );

        var result = await service.UpdateTaskAsync(completedTask.Id, updateTaskDto);
        Assert.Equal(UpdateTaskResult.CompletedTask, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_ValidInput_ShouldReturnSuccess()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var existingTask = TaskItemFactory.Create
        (
            title: "Existing Task", // Valid Title
            progress: 0 // NotStarted
        );

        context.TaskItems.Add(existingTask);
        await context.SaveChangesAsync();

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: "Updated Task Title"
        );

        var result = await service.UpdateTaskAsync(existingTask.Id, updateTaskDto);
        Assert.Equal(UpdateTaskResult.Success, result);
    }

    // GetAllTasksAsync Filtering Tests

    [Fact]
    public async Task GetAllTasksAsync_WithTagFilterAny_ShouldReturnTasksWithAtleastOneTag()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var tagWork = new Tag { Name = "Work" };
        var tagUrgent = new Tag { Name = "Urgent" };

        var task1 = TaskItemFactory.Create(title: "Task 1");
        task1.TaskTags.Add(new TaskTag { Tag = tagWork });

        var task2 = TaskItemFactory.Create(title: "Task 2");
        task2.TaskTags.Add(new TaskTag { Tag = tagUrgent });

        var task3 = TaskItemFactory.Create(title: "Task 3"); // No tags

        context.AddRange(task1, task2, task3);
        await context.SaveChangesAsync();

        var query = new TaskQueryDto
        {
            Page = 1,
            PageSize = 10,
            Tags = new List<int> { tagWork.Id, tagUrgent.Id },
            Mode = TagFilterMode.Any
        };

        var result = await service.GetAllTasksAsync(query);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Tasks, t =>
            Assert.Contains(t.Tags, tag => 
                tag.Id == tagWork.Id || tag.Id == tagUrgent.Id));

    }

    [Fact]
    public async Task GetTaskAsync_WithTagFilterAll_ShouldReturnTasksWithAllTags()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var tagWork = new Tag { Name = "Work" };
        var tagUrgent = new Tag { Name = "Urgent" };

        var task1 = TaskItemFactory.Create(title: "Task 1"); // Has both tags
        task1.TaskTags.Add(new TaskTag { Tag = tagWork });
        task1.TaskTags.Add(new TaskTag { Tag = tagUrgent });

        var task2 = TaskItemFactory.Create(title: "Task 2");
        task2.TaskTags.Add(new TaskTag { Tag = tagWork });

        context.AddRange(task1, task2);
        await context.SaveChangesAsync();

        var query = new TaskQueryDto
        {
            Page = 1,
            PageSize = 10,
            Tags = new List<int> { tagWork.Id, tagUrgent.Id },
            Mode = TagFilterMode.All
        };

        var result = await service.GetAllTasksAsync(query);
        Assert.Equal(1, result.TotalCount);

        var requiredTagIds = new[] { tagWork.Id, tagUrgent.Id };

        Assert.All(result.Tasks, t =>
        {
            var taskTagIds = t.Tags.Select(tag => tag.Id);
            Assert.True(requiredTagIds.All(id => taskTagIds.Contains(id)));
        });
    }

    [Fact]
    public async Task GetAllTasksAsync_WithoutTagFilter_ShouldReturnAllTasks()
    {
        using var context = TestDbContextFactory.Create();
        var service = new TaskService(context);

        var task1 = TaskItemFactory.Create(title: "Task 1");
        var task2 = TaskItemFactory.Create(title: "Task 2");

        context.AddRange(task1, task2);
        await context.SaveChangesAsync();

        var query = new TaskQueryDto
        {
            Page = 1,
            PageSize = 10,
            Tags = new List<int>() // empty
        };

        var result = await service.GetAllTasksAsync(query);
        Assert.Equal(2, result.TotalCount);
    }

}
