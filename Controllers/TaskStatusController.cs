using FluentValidation;
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
    private readonly IValidator<TaskStatus_SPM_Create_DTO> _createValidator;
    private readonly IValidator<TaskStatus_SPM_Update_DTO> _updateValidator;

    public TaskStatusController(
        AppDbContext context,
        IValidator<TaskStatus_SPM_Create_DTO> createValidator,
        IValidator<TaskStatus_SPM_Update_DTO> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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
            return NotFound(new ApiResponse<TaskStatus_SPM_Response_DTO> { Success = false, Message = "Task Status not found" });

        var response = new TaskStatus_SPM_Response_DTO
        {
            TaskStatusID = status.TaskStatusID,
            TaskStatusName = status.TaskStatusName,
            TaskStatusCssClass = status.TaskStatusCssClass
        };

        return Ok(new ApiResponse<TaskStatus_SPM_Response_DTO>
        {
            Success = true,
            Message = "Task Status Retrieved Successfully",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskStatus_SPM_Create_DTO dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid status data" });

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

            return Ok(new ApiResponse<TaskStatus_SPM_Response_DTO>
            {
                Success = true,
                Message = "Task Status Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding task status",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TaskStatus_SPM_Update_DTO dto)
    {
        if (dto == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid status data" });

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

        var oldStatus = await _context.TaskStatuses.FindAsync(id);

        if (oldStatus == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Task Status not found" });

        oldStatus.TaskStatusName = dto.TaskStatusName;
        oldStatus.TaskStatusCssClass = dto.TaskStatusCssClass;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Task Status Updated Successfully",
            Data = "Updated"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var status = await _context.TaskStatuses.FindAsync(id);

        if (status == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Task Status not found" });

        _context.TaskStatuses.Remove(status);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Task Status Deleted Successfully",
            Data = "Deleted"
        });
    }
}