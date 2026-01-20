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
    private readonly TaskService _service;
    private readonly AppDbContext _context;

    public TaskServiceFactTests()
    {
        _context = TestDbContextFactory.Create();
        _service = new TaskService(_context);
    }

    [Fact]
    public async Task CreateTaskAsync_DuplicateTitle_ShouldReturnDuplicateTaskTitle()
    {
        var existingTask = TaskItemFactory.Create
        (
            title: "Test Task"
        );

        _context.TaskItems.Add(existingTask);
        await _context.SaveChangesAsync();

        var newTaskDto = CreateTaskDtoFactory.Create
        (
            title: "Test Task" // Duplicate title
        );

        var result = await _service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.DuplicateTaskTitle, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTaskAsync_DueDateInPast_ShouldReturnInvalidDueDate()
    {
        var newTaskDto = CreateTaskDtoFactory.Create
        (
            dueDate: DateTime.UtcNow.AddDays(-1) // Due date in the past
        );

        var result = await _service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.DueDateInPast, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTaskAsync_InvalidInput_ShouldReturnInvalidInput()
    {
        var newTaskDto = CreateTaskDtoFactory.Create
        (
            title: " " // Invalid title (whitespace)
        );

        var result = await _service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.ValidationFailed, result.Result);
        Assert.Null(result.Created);
    }

    [Fact]
    public async Task CreateTaskAsync_ValidInput_ShouldReturnSuccess()
    {
        var newTaskDto = CreateTaskDtoFactory.Create
        (
            title: "New Valid Task", // Valid title
            dueDate: DateTime.UtcNow.AddDays(5) // Valid due date
        );

        var result = await _service.CreateTaskAsync(newTaskDto);

        Assert.Equal(CreateTaskResult.Success, result.Result);
        Assert.NotNull(result.Created);
        Assert.Equal(newTaskDto.Title, result.Created.Title);
        Assert.Equal(newTaskDto.Priority, result.Created.Priority);
        Assert.Equal(newTaskDto.DueDate, result.Created.DueDate);
    }

    [Theory]
    [InlineData(999, 1)]
    [InlineData(1, 999)]
    public async Task AddTagToTaskAsync_TaskOrTagDoesNotExist_ShouldReturnNotFound(int taskId, int tagId)
    {
        var result = await _service.AddTagToTaskAsync(taskId, tagId);
        Assert.Equal(AddTagToTaskResult.TaskNotFound, result);
    }

    [Fact]
    public async Task AddTagToTaskAsync_TagAlreadyAdded_ShouldReturnTagAlreadyAdded()
    {
        var task = TaskItemFactory.Create
        (
            title: "Task with Tag"
        );

        var tag = new Tag
        {
            Name = "Important"
        };

        _context.TaskItems.Add(task);
        _context.Tags.Add(tag);

        await _context.SaveChangesAsync();

        await _service.AddTagToTaskAsync(task.Id, tag.Id);

        var result = await _service.AddTagToTaskAsync(task.Id, tag.Id);
        Assert.Equal(AddTagToTaskResult.TagAlreadyAdded, result);
    }

    [Fact]
    public async Task AddTagToTaskAsync_ValidInput_ShouldReturnSuccess()
    {
        var task = TaskItemFactory.Create
        (
            title: "New Task with Tag"
        );

        var tag = new Tag
        {
            Name = "Urgent"
        };

        _context.TaskItems.Add(task);
        _context.Tags.Add(tag);

        await _context.SaveChangesAsync();

        var result = await _service.AddTagToTaskAsync(task.Id, tag.Id);
        Assert.Equal(AddTagToTaskResult.Success, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_NonExistentTask_ShouldReturnNotFound()
    {
        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: "Updated Title"
        );

        var result = await _service.UpdateTaskAsync(999, updateTaskDto);
        Assert.Equal(UpdateTaskResult.NotFound, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_InvalidInput_ShouldReturnValidationFailed()
    {
        var existingTask = TaskItemFactory.Create
        (
            title: "Existing Task"
        );

        _context.TaskItems.Add(existingTask);
        await _context.SaveChangesAsync();

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: " " // Invalid title (whitespace)
        );

        var result = await _service.UpdateTaskAsync(existingTask.Id, updateTaskDto);
        Assert.Equal(UpdateTaskResult.ValidationFailed, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_CompletedTask_ShouldReturnCannotUpdateCompletedTask()
    {
        var completedTask = TaskItemFactory.Create
        (
            title: "Completed Task",
            progress: 2 // Completed
        );

        _context.TaskItems.Add(completedTask);
        await _context.SaveChangesAsync();

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: "Attempt to Update Completed Task"
        );

        var result = await _service.UpdateTaskAsync(completedTask.Id, updateTaskDto);
        Assert.Equal(UpdateTaskResult.CompletedTask, result);
    }

    [Fact]
    public async Task UpdateTaskAsync_ValidInput_ShouldReturnSuccess()
    {
        var existingTask = TaskItemFactory.Create
        (
            title: "Existing Task", // Valid Title
            progress: 0 // NotStarted
        );

        _context.TaskItems.Add(existingTask);
        await _context.SaveChangesAsync();

        var updateTaskDto = UpdateTaskDtoFactory.Create
        (
            title: "Updated Task Title"
        );

        var result = await _service.UpdateTaskAsync(existingTask.Id, updateTaskDto);
        Assert.Equal(UpdateTaskResult.Success, result);
    }
}
