using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace SPM.Controllers
{
    [Route("api/priorities")]
    [ApiController]
    public class TaskPriorityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskPriorityController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TaskPriority
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskPriority>>> GetTaskPriorities()
        {
            var taskPriorities = await _context.TaskPriorities.ToListAsync();
            return Ok(taskPriorities);
        }

        // GET: api/TaskPriority/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskPriority>> GetTaskPriority(int id)
        {
            var taskPriority = await _context.TaskPriorities.FindAsync(id);

            if (taskPriority == null)
            {
                return NotFound("Task Priority not found.");
            }

            return Ok(taskPriority);
        }

        // POST: api/TaskPriority
        [HttpPost]
        public async Task<ActionResult<TaskPriority>> AddTaskPriority(TaskPriority taskPriority)
        {
            taskPriority.TaskPriorityID = 0;
            _context.TaskPriorities.Add(taskPriority);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskPriority),
                new { id = taskPriority.TaskPriorityID }, taskPriority);
        }

        // PUT: api/TaskPriority/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskPriority(int id, TaskPriority taskPriority)
        {
            if (id != taskPriority.TaskPriorityID)
            {
                return BadRequest("Task Priority ID mismatch.");
            }

            var existingTaskPriority = await _context.TaskPriorities.FindAsync(id);

            if (existingTaskPriority == null)
            {
                return NotFound("Task Priority not found.");
            }

            existingTaskPriority.TaskPriorityName = taskPriority.TaskPriorityName;
            existingTaskPriority.TaskPriorityCssClass = taskPriority.TaskPriorityCssClass;

            await _context.SaveChangesAsync();

            return Ok("Task Priority updated successfully.");
        }

        // DELETE: api/TaskPriority/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskPriority(int id)
        {
            var taskPriority = await _context.TaskPriorities.FindAsync(id);

            if (taskPriority == null)
            {
                return NotFound("Task Priority not found.");
            }

            _context.TaskPriorities.Remove(taskPriority);
            await _context.SaveChangesAsync();

            return Ok("Task Priority deleted successfully.");
        }
    }
}