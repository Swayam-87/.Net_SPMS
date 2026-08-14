using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

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
        var statuses = await _context.TaskStatuses
            .Select(ts => new TaskStatus_SPM_Response_DTO
            {
                TaskStatusID = ts.TaskStatusID,
                TaskStatusName = ts.TaskStatusName,
                TaskStatusCssClass = ts.TaskStatusCssClass
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<TaskStatus_SPM_Response_DTO>>
        {
            Success = true,
            Message = "Task Statuses Retrieved Successfully",
            Data = statuses,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskStatus(int id)
    {
        var status = await _context.TaskStatuses.FindAsync(id);

        if (status == null)
            return NotFound();

        var response = new TaskStatus_SPM_Response_DTO
        {
            TaskStatusID = status.TaskStatusID,
            TaskStatusName = status.TaskStatusName,
            TaskStatusCssClass = status.TaskStatusCssClass
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskStatus_SPM_Create_DTO dto)
    {
        var status = new TaskStatus_SPM
        {
            TaskStatusName = dto.TaskStatusName,
            TaskStatusCssClass = dto.TaskStatusCssClass
        };

        _context.TaskStatuses.Add(status);
        await _context.SaveChangesAsync();

        var response = new TaskStatus_SPM_Response_DTO
        {
            TaskStatusID = status.TaskStatusID,
            TaskStatusName = status.TaskStatusName,
            TaskStatusCssClass = status.TaskStatusCssClass
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskStatus_SPM_Update_DTO dto)
    {
        var oldStatus = await _context.TaskStatuses.FindAsync(id);

        if (oldStatus == null)
            return NotFound();

        oldStatus.TaskStatusName = dto.TaskStatusName;
        oldStatus.TaskStatusCssClass = dto.TaskStatusCssClass;
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