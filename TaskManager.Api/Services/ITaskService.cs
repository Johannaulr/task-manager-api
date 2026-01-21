using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Api.DTOs;
using TaskManager.Api.Models;
using TaskManager.Api.Services.TaskResults;

namespace TaskManager.Api.Services
{
    public interface ITaskService
    {
        Task<(CreateTaskResult Result, TaskDto? Created)> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<AddTagToTaskResult> AddTagToTaskAsync(int taskId, int tagId);
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task<PagedResult<TaskDto>> GetAllTasksAsync(TaskQueryDto queryDto);
        Task<UpdateTaskResult> UpdateTaskAsync(int id, UpdateTaskDto taskDto);
        Task<DeleteTaskResult> DeleteTaskAsync(int id);
    }
}