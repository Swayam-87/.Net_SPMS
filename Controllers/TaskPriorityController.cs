using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

[ApiController]
[Route("api/[controller]")]
public class TaskPriorityController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskPriorityController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskPriorities()
    {
        var priorities = await _context.TaskPriorities.ToListAsync();
        return Ok(priorities);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskPriority(int id)
    {
        var priority = await _context.TaskPriorities.FindAsync(id);

        if (priority == null)
            return NotFound();

        return Ok(priority);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskPriority priority)
    {
        _context.TaskPriorities.Add(priority);
        await _context.SaveChangesAsync();

        return Ok(priority);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskPriority priority)
    {
        if (id != priority.TaskPriorityID)
            return BadRequest();

        var oldPriority = await _context.TaskPriorities.FindAsync(id);

        oldPriority.TaskPriorityName = priority.TaskPriorityName;
        oldPriority.TaskPriorityCssClass = priority.TaskPriorityCssClass;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var priority = await _context.TaskPriorities.FindAsync(id);

        if (priority == null)
            return NotFound();

        _context.TaskPriorities.Remove(priority);
        await _context.SaveChangesAsync();

        return Ok();
    }
}