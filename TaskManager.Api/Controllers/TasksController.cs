using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using TaskManager.Api.DTOs;
using TaskManager.Api.Services.TaskResults;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        // POST /api/tasks
        // GET /api/tasks
        // GET /api/tasks/{id}
        // PUT /api/tasks/{id}
        // DELETE /api/tasks/{id}

        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto taskDto)
        {
            var (result, task) = await _taskService.CreateTaskAsync(taskDto);

            return result switch
            {
                CreateTaskResult.Success => CreatedAtAction(nameof(GetTaskByIdAsync), 
                    new { id = task.Id }, task),
                CreateTaskResult.DuplicateTaskTitle => Conflict("A task with this title already exists"),
                CreateTaskResult.ValidationFailed => BadRequest("Invalid task data"),
                CreateTaskResult.DueDateInPast => BadRequest("Due date can not be in the past"),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TaskDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetTaskByIdAsync(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);

                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An unexpected error occurred: {e.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TaskItem>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();

                if (!tasks.Any())
                    return NoContent(); //204

                return Ok(tasks); //200
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"An unexpected error occurred: {e.Message}");
            }

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto taskDto)
        {
            var result = await _taskService.UpdateTaskAsync(id, taskDto);

            return result switch
            {
                UpdateTaskResult.Success => NoContent(),
                UpdateTaskResult.NotFound => NotFound(),
                UpdateTaskResult.ValidationFailed => BadRequest("Invalid task data"),
                UpdateTaskResult.CompletedTask => BadRequest("Can not edit completed tasks"),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            
            return result switch
            {
                DeleteTaskResult.Success => NoContent(),
                DeleteTaskResult.NotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

    }
}