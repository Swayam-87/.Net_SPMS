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

    public UserRoleController(AppDbContext context)
    {
        _context = context;
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
            return NotFound();

        var response = new UserRole_Admin_Response_DTO
        {
            RolePermissionID = userRole.RolePermissionID,
            RoleID = userRole.RoleID,
            RoleName = userRole.Role?.RoleName ?? "",
            UserID = userRole.UserID,
            UserName = userRole.User?.FullName ?? ""
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserRole_Create_DTO dto)
    {
        try
        {
            var userRole = new UserRole
            {
                RoleID = dto.RoleID,
                UserID = dto.UserID
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            // Reload with navigation properties for response
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
        var oldUserRole = await _context.UserRoles.FindAsync(id);

        if (oldUserRole == null)
            return NotFound();

        oldUserRole.RoleID = dto.RoleID;
        oldUserRole.UserID = dto.UserID;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);

        if (userRole == null)
            return NotFound();

        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();

        return Ok();
    }
}