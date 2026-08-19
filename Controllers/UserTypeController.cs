using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

[ApiController]
[Route("api/[controller]")]
public class UserTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserTypeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetUserTypes")]
    public async Task<IActionResult> GetUserTypes()
    {
        var userTypes = await _context.UserTypes
            .Select(ut => new UserType_Admin_Response_DTO
            {
                UserTypeID = ut.UserTypeID,
                UserTypeName = ut.UserTypeName,
                Description = ut.Description
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<UserType_Admin_Response_DTO>>
        {
            Success = true,
            Message = "User Types Retrieved Successfully",
            Data = userTypes,
        });
    }

    [HttpGet("GetUserTypeById/{id}")]
    public async Task<IActionResult> GetUserType(int id)
    {
        var userType = await _context.UserTypes.FindAsync(id);

        if (userType == null)
            return NotFound(new ApiResponse<UserType_Admin_Response_DTO> { Success = false, Message = "User Type not found" });

        var response = new UserType_Admin_Response_DTO
        {
            UserTypeID = userType.UserTypeID,
            UserTypeName = userType.UserTypeName,
            Description = userType.Description
        };

        return Ok(new ApiResponse<UserType_Admin_Response_DTO>
        {
            Success = true,
            Message = "User Type Retrieved Successfully",
            Data = response
        });
    }

    
    [HttpPost("CreateUserType")]
    public async Task<IActionResult> Create(UserType_Create_DTO dto)
    {
        try
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserTypeName))
                return BadRequest(new ApiResponse<object> { Success = false, Message = "User Type Name is required" });

            var exists = await _context.UserTypes.AnyAsync(ut => ut.UserTypeName.ToLower() == dto.UserTypeName.Trim().ToLower());
            if (exists)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "A user type with this name already exists" });

            var userType = new UserType
            {
                UserTypeName = dto.UserTypeName.Trim(),
                Description = dto.Description ?? ""
            };

            _context.UserTypes.Add(userType);
            await _context.SaveChangesAsync();

            var response = new UserType_Admin_Response_DTO
            {
                UserTypeID = userType.UserTypeID,
                UserTypeName = userType.UserTypeName,
                Description = userType.Description
            };

            return Ok(new ApiResponse<UserType_Admin_Response_DTO>
            {
                Success = true,
                Message = "User Type Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding user type",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("UpdateUserType/{id}")]
    public async Task<IActionResult> Update(int id, UserType_Update_DTO dto)
    {
        var oldUserType = await _context.UserTypes.FindAsync(id);

        if (oldUserType == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "User Type not found" });

        if (dto != null)
        {
            if (!string.IsNullOrWhiteSpace(dto.UserTypeName))
            {
                var exists = await _context.UserTypes.AnyAsync(ut => ut.UserTypeID != id && ut.UserTypeName.ToLower() == dto.UserTypeName.Trim().ToLower());
                if (exists)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "A user type with this name already exists" });

                oldUserType.UserTypeName = dto.UserTypeName.Trim();
            }

            if (dto.Description != null)
                oldUserType.Description = dto.Description;

            await _context.SaveChangesAsync();
        }

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User Type Updated Successfully",
            Data = "Updated"
        });
    }

   
    [HttpDelete("DeleteUserType/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userType = await _context.UserTypes.FindAsync(id);

        if (userType == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "User Type not found" });

        var hasAssignedUsers = await _context.Users.AnyAsync(u => u.UserTypeID == id && (u.IsDeleted == null || u.IsDeleted == false));
        if (hasAssignedUsers)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "Cannot delete this User Type because active users are currently assigned to it."
            });
        }

        _context.UserTypes.Remove(userType);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User Type Deleted Successfully",
            Data = "Deleted"
        });
    }
}