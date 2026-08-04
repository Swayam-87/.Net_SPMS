using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace SPM.Controllers
{
    [Route("api/statuses")]
    [ApiController]
    public class TaskStatusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskStatusController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TaskStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskStatus_SPM>>> GetTaskStatuses()
        {
            var taskStatuses = await _context.TaskStatuses.ToListAsync();
            return Ok(taskStatuses);
        }

        // GET: api/TaskStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskStatus_SPM>> GetTaskStatus(int id)
        {
            var taskStatus = await _context.TaskStatuses.FindAsync(id);

            if (taskStatus == null)
            {
                return NotFound("Task Status not found.");
            }

            return Ok(taskStatus);
        }

        // POST: api/TaskStatus
        [HttpPost]
        public async Task<ActionResult<TaskStatus>> AddTaskStatus(TaskStatus_SPM taskStatus)
        {
            taskStatus.TaskStatusID = 0;
            _context.TaskStatuses.Add(taskStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskStatus),
                new { id = taskStatus.TaskStatusID }, taskStatus);
        }

        // PUT: api/TaskStatus/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, TaskStatus_SPM taskStatus)
        {
            if (id != taskStatus.TaskStatusID)
            {
                return BadRequest("Task Status ID mismatch.");
            }

            var existingTaskStatus = await _context.TaskStatuses.FindAsync(id);

            if (existingTaskStatus == null)
            {
                return NotFound("Task Status not found.");
            }

            existingTaskStatus.TaskStatusName = taskStatus.TaskStatusName;
            existingTaskStatus.TaskStatusCssClass = taskStatus.TaskStatusCssClass;

            await _context.SaveChangesAsync();

            return Ok("Task Status updated successfully.");
        }

   
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskStatus(int id)
        {
            var taskStatus = await _context.TaskStatuses.FindAsync(id);

            if (taskStatus == null)
            {
                return NotFound("Task Status not found.");
            }

            _context.TaskStatuses.Remove(taskStatus);
            await _context.SaveChangesAsync();

            return Ok("Task Status deleted successfully.");
        }
    }
}