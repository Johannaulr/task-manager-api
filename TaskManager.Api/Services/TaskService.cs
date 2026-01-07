using TaskManager.Api.Data;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
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

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto taskDto)
        {
            var task = new TaskItem
            {
                Title = taskDto.Title,
                Priority = (int)taskDto.Priority,
                DueDate = taskDto.DueDate,
            };

            _context.taskItems.Add(task);

            await _context.SaveChangesAsync();
            
            var taskDtoResult = MapToDto(task);
            return taskDtoResult;
        }

        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _context.taskItems.FindAsync(id);
            if (task == null) return null;

            return MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasksAsync()
        {
            var tasks = await _context.taskItems.ToListAsync();

            var taskDtos = tasks.Select(MapToDto).ToList();
            
            return taskDtos;
        }

        public async Task<bool> UpdateTaskAsync(int id, UpdateTaskDto taskDto)
        {
            var task = await _context.taskItems.FindAsync(id);
            if (task == null) return false;

            task.Title = taskDto.Title;
            task.Priority = (int)taskDto.Priority;
            task.DueDate = taskDto.DueDate;
            task.IsCompleted = taskDto.IsCompleted;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.taskItems.FindAsync(id);
            if (task == null) return false;

            _context.taskItems.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }

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
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException(int taskId)
            : base($"Task with ID {taskId} not found.") { }
    }

    public class InvalidTaskDataException : Exception
    {
        public InvalidTaskDataException(string message)
            : base(message) { }
    }

}