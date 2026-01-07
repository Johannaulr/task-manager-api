using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Models;
using TaskManager.Api.Services;
using TaskManager.Api.DTOs;

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
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto taskDto)
        {
            var task = await _taskService.CreateTaskAsync(taskDto);
            if (task == null) return BadRequest("Unable to create task");

            return CreatedAtAction(nameof(GetTaskByIdAsync), new { id = task.Id }, task);
        }

        // Action for getting a task by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskByIdAsync(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]   // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // Invalid input
        [ProducesResponseType(StatusCodes.Status404NotFound)]    // Task not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Unexpected error
        public async Task<ActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto taskDto)
        {
            var result = await _taskService.UpdateTaskAsync(id, taskDto);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]   // Success
        [ProducesResponseType(StatusCodes.Status404NotFound)]    // Task not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Unexpected error
        public async Task<ActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            if (!result) return NotFound();

            return NoContent();
            
        }

    }
}