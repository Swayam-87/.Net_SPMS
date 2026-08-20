using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

[ApiController]
[Route("api/[controller]")]
public class UserRoleController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<UserRole_Create_DTO> _createValidator;
    private readonly IValidator<UserRole_Update_DTO> _updateValidator;

    public UserRoleController(
        AppDbContext context,
        IValidator<UserRole_Create_DTO> createValidator,
        IValidator<UserRole_Update_DTO> updateValidator)
    {
        _context = context;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserRoles()
    {
        var userRoles = await _context.UserRoles
            .Include(ur => ur.Role)
            .Include(ur => ur.User)
            .Select(ur => new UserRole_Admin_Response_DTO
            {
                RolePermissionID = ur.RolePermissionID,
                RoleID = ur.RoleID,
                RoleName = ur.Role.RoleName ?? "",
                UserID = ur.UserID,
                UserName = ur.User.FullName ?? ""
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UserRole_Admin_Response_DTO>>
        {
            Success = true,
            Message = "User Roles Retrieved Successfully",
            Data = userRoles,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserRole(int id)
    {
        var userRole = await _context.UserRoles
            .Include(ur => ur.Role)
            .Include(ur => ur.User)
            .FirstOrDefaultAsync(ur => ur.RolePermissionID == id);

        if (userRole == null)
            return NotFound(new ApiResponse<UserRole_Admin_Response_DTO> { Success = false, Message = "User Role not found" });

        var response = new UserRole_Admin_Response_DTO
        {
            RolePermissionID = userRole.RolePermissionID,
            RoleID = userRole.RoleID,
            RoleName = userRole.Role?.RoleName ?? "",
            UserID = userRole.UserID,
            UserName = userRole.User?.FullName ?? ""
        };

        return Ok(new ApiResponse<UserRole_Admin_Response_DTO>
        {
            Success = true,
            Message = "User Role Retrieved Successfully",
            Data = response
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserRole_Create_DTO dto)
    {
        try
        {
            if (dto == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid user role data" });

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

            var userRole = new UserRole
            {
                RoleID = dto.RoleID,
                UserID = dto.UserID
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            await _context.Entry(userRole).Reference(ur => ur.Role).LoadAsync();
            await _context.Entry(userRole).Reference(ur => ur.User).LoadAsync();

            var response = new UserRole_Admin_Response_DTO
            {
                RolePermissionID = userRole.RolePermissionID,
                RoleID = userRole.RoleID,
                RoleName = userRole.Role?.RoleName ?? "",
                UserID = userRole.UserID,
                UserName = userRole.User?.FullName ?? ""
            };

            return Ok(new ApiResponse<UserRole_Admin_Response_DTO>
            {
                Success = true,
                Message = "User Role Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding user role",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserRole_Update_DTO dto)
    {
        if (dto == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid user role data" });

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

        var oldUserRole = await _context.UserRoles.FindAsync(id);

        if (oldUserRole == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "User Role not found" });

        oldUserRole.RoleID = dto.RoleID;
        oldUserRole.UserID = dto.UserID;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User Role Updated Successfully",
            Data = "Updated"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);

        if (userRole == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "User Role not found" });

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User Role Deleted Successfully",
            Data = "Deleted"
        });
    }
}