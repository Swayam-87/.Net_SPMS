using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectMasterController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<ProjectMaster_Create_DTO> _createValidator;
    private readonly IValidator<ProjectMaster_Update_DTO> _updateValidator;

    public ProjectMasterController(
        AppDbContext context,
        IValidator<ProjectMaster_Create_DTO> createValidator,
        IValidator<ProjectMaster_Update_DTO> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _context.ProjectMasters
            .Select(p => new ProjectMaster_Admin_Response_DTO
            {
                ProjectID = p.ProjectID,
                ProjectTitle = p.ProjectTitle,
                Description = p.Description
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<ProjectMaster_Admin_Response_DTO>>
        {
            Success = true,
            Message = "Projects Retrieved Successfully",
            Data = projects,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _context.ProjectMasters.FindAsync(id);

        if (project == null)
            return NotFound(new ApiResponse<ProjectMaster_Admin_Response_DTO> { Success = false, Message = "Project not found" });

        var response = new ProjectMaster_Admin_Response_DTO
        {
            ProjectID = project.ProjectID,
            ProjectTitle = project.ProjectTitle,
            Description = project.Description
        };

        return Ok(new ApiResponse<ProjectMaster_Admin_Response_DTO>
        {
            Success = true,
            Message = "Project Retrieved Successfully",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectMaster_Create_DTO dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid project data" });

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

            var project = new ProjectMaster
            {
                ProjectTitle = dto.ProjectTitle,
                Description = dto.Description
            };

            _context.ProjectMasters.Add(project);
            await _context.SaveChangesAsync();

            var response = new ProjectMaster_Admin_Response_DTO
            {
                ProjectID = project.ProjectID,
                ProjectTitle = project.ProjectTitle,
                Description = project.Description
            };

            return Ok(new ApiResponse<ProjectMaster_Admin_Response_DTO>
            {
                Success = true,
                Message = "Project Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding project",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProjectMaster_Update_DTO dto)
    {
        if (dto == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid project data" });

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

        var oldProject = await _context.ProjectMasters.FindAsync(id);

        if (oldProject == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Project not found" });

        oldProject.ProjectTitle = dto.ProjectTitle;
        oldProject.Description = dto.Description;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Project Updated Successfully",
            Data = "Updated"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.ProjectMasters.FindAsync(id);

        if (project == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "Project not found" });

        _context.ProjectMasters.Remove(project);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Project Deleted Successfully",
            Data = "Deleted"
        });
    }
}
