using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

[ApiController]
[Route("api/[controller]")]
public class TaskStatusController : ControllerBase
{
    private readonly AppDbContext _context;

    public TaskStatusController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskStatuses()
    {
        var statuses = await _context.TaskStatuses.ToListAsync();
        return Ok(statuses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskStatus(int id)
    {
        var status = await _context.TaskStatuses.FindAsync(id);

        if (status == null)
            return NotFound();

        return Ok(status);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskStatus_SPM status)
    {
        _context.TaskStatuses.Add(status);
        await _context.SaveChangesAsync();

        return Ok(status);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskStatus_SPM status)
    {
        if (id != status.TaskStatusID)
            return BadRequest();

        var oldStatus = await _context.TaskStatuses.FindAsync(id);

        oldStatus.TaskStatusName = status.TaskStatusName;
        oldStatus.TaskStatusCssClass = status.TaskStatusCssClass;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var status = await _context.TaskStatuses.FindAsync(id);

        if (status == null)
            return NotFound();

        _context.TaskStatuses.Remove(status);
        await _context.SaveChangesAsync();

        return Ok();
    }
}