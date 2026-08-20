using FluentValidation;
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
    private readonly IValidator<TaskPriority_Create_DTO> _createValidator;
    private readonly IValidator<TaskPriority_Update_DTO> _updateValidator;

    public TaskPriorityController(
        AppDbContext context,
        IValidator<TaskPriority_Create_DTO> createValidator,
        IValidator<TaskPriority_Update_DTO> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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
            return NotFound(new ApiResponse<TaskPriority_Response_DTO> { Success = false, Message = "Task Priority not found" });

        var response = new TaskPriority_Response_DTO
        {
            TaskPriorityID = priority.TaskPriorityID,
            TaskPriorityName = priority.TaskPriorityName,
            TaskPriorityCssClass = priority.TaskPriorityCssClass
        };

        return Ok(new ApiResponse<TaskPriority_Response_DTO>
        {
            Success = true,
            Message = "Task Priority Retrieved Successfully",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskPriority_Create_DTO dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid priority data" });

            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });
            }

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
        if (dto == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid priority data" });

        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        var oldPriority = await _context.TaskPriorities.FindAsync(id);

        if (oldPriority == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Task Priority not found" });

        oldPriority.TaskPriorityName = dto.TaskPriorityName;
        oldPriority.TaskPriorityCssClass = dto.TaskPriorityCssClass;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Task Priority Updated Successfully",
            Data = "Updated"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var priority = await _context.TaskPriorities.FindAsync(id);

        if (priority == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Task Priority not found" });

        _context.TaskPriorities.Remove(priority);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Task Priority Deleted Successfully",
            Data = "Deleted"
        });
    }
}