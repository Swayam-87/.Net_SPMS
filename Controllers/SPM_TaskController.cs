using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

[ApiController]
[Route("api/[controller]")]
public class SPM_TaskController : ControllerBase
{
    private readonly AppDbContext _context;

    public SPM_TaskController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _context.Tasks.ToListAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SPM_Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SPM_Task task)
    {
        if (id != task.TaskID)
            return BadRequest();

        var oldTask = await _context.Tasks.FindAsync(id);

        oldTask.ProjectAllocationID = task.ProjectAllocationID;
        oldTask.TaskTitle = task.TaskTitle;
        oldTask.TaskDescription = task.TaskDescription;
        oldTask.TaskStatusID = task.TaskStatusID;
        oldTask.TaskPriorityID = task.TaskPriorityID;
        oldTask.AssignedScore = task.AssignedScore;
        oldTask.EarnedScore = task.EarnedScore;
        oldTask.ProgressPercentage = task.ProgressPercentage;
        oldTask.TaskAssignedDate = task.TaskAssignedDate;
        oldTask.TaskStartDate = task.TaskStartDate;
        oldTask.TaskDueDate = task.TaskDueDate;
        oldTask.TaskCompletedDate = task.TaskCompletedDate;
        oldTask.NextFollowUpDate = task.NextFollowUpDate;
        oldTask.FacultyRemarks = task.FacultyRemarks;
        oldTask.StudentRemarks = task.StudentRemarks;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
