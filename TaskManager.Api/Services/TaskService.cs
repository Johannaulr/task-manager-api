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
            bool exists = await _context.TaskItems
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

            _context.TaskItems.Add(task);

            await _context.SaveChangesAsync();
            
            var taskDto = MapToDto(task);
            return (CreateTaskResult.Success, taskDto);
        }

        public async Task<AddTagToTaskResult> AddTagToTaskAsync(int taskId, int tagId)
        {
            var taskExists = await _context.TaskItems.AnyAsync(t => t.Id == taskId);
            if (!taskExists)
                return AddTagToTaskResult.TaskNotFound;
            
            var tagExists = await _context.Tags.AnyAsync(t => t.Id == tagId);
            if (!tagExists)
                return AddTagToTaskResult.TagNotFound;
            
            var alreadyAssigned = await _context.TaskTags
                .AnyAsync(tt => tt.TaskItemId == taskId && tt.TagId == tagId);
            if (alreadyAssigned)
                return AddTagToTaskResult.TagAlreadyAdded;
            
            var taskTag = new TaskTag
            {
                TaskItemId = taskId,
                TagId = tagId
            };

            _context.TaskTags.Add(taskTag);
            await _context.SaveChangesAsync();
            
            return AddTagToTaskResult.Success;
        }

        // READ
        public async Task<TaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _context.TaskItems
            .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == id);

            return task == null ? null : MapToDto(task);
        }

        public async Task<PagedResult<TaskDto>> GetAllTasksAsync(int pageNumber, int pageSize)
        {

            var query = _context.TaskItems.AsNoTracking();
            var totalCount = await query.CountAsync();

            var tasks = await query
                .OrderBy(t => t.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Priority = (TaskPriority)t.Priority,
                    DueDate = t.DueDate,
                    Progress = (TaskProgress)t.Progress,
                    Tags = t.TaskTags.Select(tt => new TagDto
                    {
                        Id = tt.Tag.Id,
                        Name = tt.Tag.Name
                    }).ToList()
                })
                .ToListAsync();


            return new PagedResult<TaskDto>
            {
                Tasks = tasks,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        // UPDATE
        public async Task<UpdateTaskResult> UpdateTaskAsync(int id, UpdateTaskDto taskDto)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null) 
                return UpdateTaskResult.NotFound;

            // Business rule: Completed task cannot be edited
            if (task.Progress == 2) 
                return UpdateTaskResult.CompletedTask;

            if (string.IsNullOrWhiteSpace(taskDto.Title)) 
                return UpdateTaskResult.ValidationFailed;

            task.Title = taskDto.Title;
            task.Priority = (int)taskDto.Priority;
            task.DueDate = taskDto.DueDate;
            task.Progress = (int)taskDto.Progress;

            await _context.SaveChangesAsync();

            return UpdateTaskResult.Success;
        }

        // DELETE
        public async Task<DeleteTaskResult> DeleteTaskAsync(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null) 
                return DeleteTaskResult.NotFound;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return DeleteTaskResult.Success;
        }

        // MAPPER
        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Priority = (TaskPriority)task.Priority,
                DueDate = task.DueDate,
                Progress = (TaskProgress)task.Progress,
                Tags = task.TaskTags?.Select(tt => new TagDto
                {
                    Id = tt.Tag.Id,
                    Name = tt.Tag.Name
                }).ToList() ?? new List<TagDto>()
            };
        }
    }
}