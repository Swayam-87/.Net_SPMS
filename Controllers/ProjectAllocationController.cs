using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectAllocationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<ProjectAllocation_Create_DTO> _createValidator;
    private readonly IValidator<ProjectAllocation_Update_DTO> _updateValidator;

    public ProjectAllocationController(
        AppDbContext context,
        IValidator<ProjectAllocation_Create_DTO> createValidator,
        IValidator<ProjectAllocation_Update_DTO> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectAllocations()
    {
        var allocations = await _context.ProjectAllocations
            .Include(pa => pa.Project)
            .Include(pa => pa.Student)
            .Include(pa => pa.Faculty)
            .Select(pa => new ProjectAllocation_Admin_Response_DTO
            {
                ProjectAllocationID = pa.ProjectAllocationID,
                ProjectID = pa.ProjectID,
                ProjectTitle = pa.Project.ProjectTitle ?? "",
                StudentID = pa.StudentID,
                StudentName = pa.Student.FullName ?? "",
                FacultyID = pa.FacultyID,
                FacultyName = pa.Faculty.FullName ?? "",
                AssignedDate = pa.AssignedDate,
                ProjectStartDate = pa.ProjectStartDate,
                ProjectEndDate = pa.ProjectEndDate,
                TotalTasksGiven = pa.TotalTasksGiven,
                TotalCompletedTasks = pa.TotalCompletedTasks,
                ProgressPercentage = pa.ProgressPercentage,
                OverAllGrade = pa.OverAllGrade
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<ProjectAllocation_Admin_Response_DTO>>
        {
            Success = true,
            Message = "Project Allocations Retrieved Successfully",
            Data = allocations,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectAllocation(int id)
    {
        var allocation = await _context.ProjectAllocations
            .Include(pa => pa.Project)
            .Include(pa => pa.Student)
            .Include(pa => pa.Faculty)
            .FirstOrDefaultAsync(pa => pa.ProjectAllocationID == id);

        if (allocation == null)
            return NotFound(new ApiResponse<ProjectAllocation_Admin_Response_DTO> { Success = false, Message = "Project Allocation not found" });

        var response = new ProjectAllocation_Admin_Response_DTO
        {
            ProjectAllocationID = allocation.ProjectAllocationID,
            ProjectID = allocation.ProjectID,
            ProjectTitle = allocation.Project?.ProjectTitle ?? "",
            StudentID = allocation.StudentID,
            StudentName = allocation.Student?.FullName ?? "",
            FacultyID = allocation.FacultyID,
            FacultyName = allocation.Faculty?.FullName ?? "",
            AssignedDate = allocation.AssignedDate,
            ProjectStartDate = allocation.ProjectStartDate,
            ProjectEndDate = allocation.ProjectEndDate,
            TotalTasksGiven = allocation.TotalTasksGiven,
            TotalCompletedTasks = allocation.TotalCompletedTasks,
            ProgressPercentage = allocation.ProgressPercentage,
            OverAllGrade = allocation.OverAllGrade
        };

        return Ok(new ApiResponse<ProjectAllocation_Admin_Response_DTO>
        {
            Success = true,
            Message = "Project Allocation Retrieved Successfully",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectAllocation_Create_DTO dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid allocation data" });

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

            var allocation = new ProjectAllocation
            {
                ProjectID = dto.ProjectID,
                StudentID = dto.StudentID,
                FacultyID = dto.FacultyID,
                AssignedDate = DateTime.Now,
                ProjectStartDate = dto.ProjectStartDate,
                ProjectEndDate = dto.ProjectEndDate
            };

            _context.ProjectAllocations.Add(allocation);
            await _context.SaveChangesAsync();

            await _context.Entry(allocation).Reference(pa => pa.Project).LoadAsync();
            await _context.Entry(allocation).Reference(pa => pa.Student).LoadAsync();
            await _context.Entry(allocation).Reference(pa => pa.Faculty).LoadAsync();

            var response = new ProjectAllocation_Admin_Response_DTO
            {
                ProjectAllocationID = allocation.ProjectAllocationID,
                ProjectID = allocation.ProjectID,
                ProjectTitle = allocation.Project?.ProjectTitle ?? "",
                StudentID = allocation.StudentID,
                StudentName = allocation.Student?.FullName ?? "",
                FacultyID = allocation.FacultyID,
                FacultyName = allocation.Faculty?.FullName ?? "",
                AssignedDate = allocation.AssignedDate,
                ProjectStartDate = allocation.ProjectStartDate,
                ProjectEndDate = allocation.ProjectEndDate,
                TotalTasksGiven = allocation.TotalTasksGiven,
                TotalCompletedTasks = allocation.TotalCompletedTasks,
                ProgressPercentage = allocation.ProgressPercentage,
                OverAllGrade = allocation.OverAllGrade
            };

            return Ok(new ApiResponse<ProjectAllocation_Admin_Response_DTO>
            {
                Success = true,
                Message = "Project Allocation Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding project allocation",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProjectAllocation_Update_DTO dto)
    {
        if (dto == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid allocation data" });

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

        var oldAllocation = await _context.ProjectAllocations.FindAsync(id);

        if (oldAllocation == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Project Allocation not found" });

        oldAllocation.ProjectID = dto.ProjectID;
        oldAllocation.StudentID = dto.StudentID;
        oldAllocation.FacultyID = dto.FacultyID;
        oldAllocation.AssignedDate = dto.AssignedDate;
        oldAllocation.ProjectStartDate = dto.ProjectStartDate;
        oldAllocation.ProjectEndDate = dto.ProjectEndDate;
        oldAllocation.TotalTasksGiven = dto.TotalTasksGiven;
        oldAllocation.TotalCompletedTasks = dto.TotalCompletedTasks;
        oldAllocation.ProgressPercentage = dto.ProgressPercentage;
        oldAllocation.OverAllGrade = dto.OverAllGrade;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Project Allocation Updated Successfully",
            Data = "Updated"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var allocation = await _context.ProjectAllocations.FindAsync(id);

        if (allocation == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Project Allocation not found" });

        _context.ProjectAllocations.Remove(allocation);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Project Allocation Deleted Successfully",
            Data = "Deleted"
        });
    }
}
