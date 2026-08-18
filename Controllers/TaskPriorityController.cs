using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

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
        var priorities = await _context.TaskPriorities
            .Select(tp => new TaskPriority_Response_DTO
            {
                TaskPriorityID = tp.TaskPriorityID,
                TaskPriorityName = tp.TaskPriorityName,
                TaskPriorityCssClass = tp.TaskPriorityCssClass
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<TaskPriority_Response_DTO>>
        {
            Success = true,
            Message = "Task Priorities Retrieved Successfully",
            Data = priorities,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskPriority(int id)
    {
        var priority = await _context.TaskPriorities.FindAsync(id);

        if (priority == null)
            return NotFound();

        var response = new TaskPriority_Response_DTO
        {
            TaskPriorityID = priority.TaskPriorityID,
            TaskPriorityName = priority.TaskPriorityName,
            TaskPriorityCssClass = priority.TaskPriorityCssClass
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskPriority_Create_DTO dto)
    {
        try
        {
            var priority = new TaskPriority
            {
                TaskPriorityName = dto.TaskPriorityName,
                TaskPriorityCssClass = dto.TaskPriorityCssClass
            };

            _context.TaskPriorities.Add(priority);
            await _context.SaveChangesAsync();

            var response = new TaskPriority_Response_DTO
            {
                TaskPriorityID = priority.TaskPriorityID,
                TaskPriorityName = priority.TaskPriorityName,
                TaskPriorityCssClass = priority.TaskPriorityCssClass
            };

            return Ok(new ApiResponse<TaskPriority_Response_DTO>
            {
                Success = true,
                Message = "Task Priority Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding task priority",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskPriority_Update_DTO dto)
    {
        var oldPriority = await _context.TaskPriorities.FindAsync(id);

        if (oldPriority == null)
            return NotFound();

        oldPriority.TaskPriorityName = dto.TaskPriorityName;
        oldPriority.TaskPriorityCssClass = dto.TaskPriorityCssClass;
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