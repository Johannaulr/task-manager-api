using TaskManager.Api.Data;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
using TaskManager.Api.Services.TaskResults;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        public TaskService(AppDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<(CreateTaskResult Result, TaskDto? Created)> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            // Business rule: duplicate title
            bool exists = await _context.taskItems
                .AnyAsync(t => t.Title == createTaskDto.Title);

            if (exists)
                return (CreateTaskResult.DuplicateTaskTitle, null);

            if (string.IsNullOrWhiteSpace(createTaskDto.Title))
                return (CreateTaskResult.ValidationFailed, null);
            
            if (createTaskDto.DueDate < DateTime.UtcNow)
                return (CreateTaskResult.DueDateInPast, null);

            var task = new TaskItem
            {
                Title = createTaskDto.Title,
                Priority = (int)createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
            };

            _context.taskItems.Add(task);

            await _context.SaveChangesAsync();
            
            var taskDto = MapToDto(task);
            return (CreateTaskResult.Success, taskDto);
        }

        // READ
        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _context.taskItems.FindAsync(id);
            return task == null ? null : MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _context.taskItems.ToListAsync();
            return tasks.Select(MapToDto);
        }

        // UPDATE
        public async Task<UpdateTaskResult> UpdateTaskAsync(int id, UpdateTaskDto taskDto)
        {
            var task = await _context.taskItems.FindAsync(id);

            if (task == null) 
                return UpdateTaskResult.NotFound;

            // Business rule: Completed task cannot be edited
            if (task.IsCompleted) 
                return UpdateTaskResult.CompletedTask;

            if (string.IsNullOrWhiteSpace(taskDto.Title)) 
                return UpdateTaskResult.ValidationFailed;

            task.Title = taskDto.Title;
            task.Priority = (int)taskDto.Priority;
            task.DueDate = taskDto.DueDate;
            task.IsCompleted = taskDto.IsCompleted;

            await _context.SaveChangesAsync();

            return UpdateTaskResult.Success;
        }

        // DELETE
        public async Task<DeleteTaskResult> DeleteTaskAsync(int id)
        {
            var task = await _context.taskItems.FindAsync(id);
            if (task == null) return DeleteTaskResult.NotFound;

            _context.taskItems.Remove(task);
            await _context.SaveChangesAsync();

            return DeleteTaskResult.Success;
        }

        // MAPPER
        private TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Priority = (TaskPriority)task.Priority,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
            };
        }
    }
}