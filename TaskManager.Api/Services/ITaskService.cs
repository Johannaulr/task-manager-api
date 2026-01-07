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
        Task<TaskDto> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskDto>> GetAllTasksAsync();
        Task<UpdateTaskResult> UpdateTaskAsync(int id, UpdateTaskDto taskDto);
        Task<DeleteTaskResult> DeleteTaskAsync(int id);
    }
}