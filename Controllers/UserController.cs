using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Select(u => new User_Admin_Response_DTO
            {
                UserID = u.UserID,
                UserTypeID = u.UserTypeID,
                UserTypeName = u.UserType != null ? u.UserType.UserTypeName : "",
                FullName = u.FullName,
                UserCode = u.UserCode,
                Email = u.Email,
                Password = u.Password,
                MobileNumber = u.MobileNumber,
                ProfilePicturePath = u.ProfilePicturePath,
                IsActive = u.IsActive,
                IsDeleted = u.IsDeleted
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<User_Admin_Response_DTO>>
        {
            Success = true,
            Message = "Users Retrieved Successfully",
            Data = users
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return NotFound(new ApiResponse<User_Admin_Response_DTO> { Success = false, Message = "User not found" });

        return Ok(new ApiResponse<User_Admin_Response_DTO>
        {
            Success = true,
            Message = "User Retrieved Successfully",
            Data = MapToDto(user)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(User_Create_DTO dto)
    {
        try
        {
            var user = new User
            {
                UserTypeID = dto.UserTypeID,
                FullName = dto.FullName ?? "",
                UserCode = !string.IsNullOrWhiteSpace(dto.UserCode) ? dto.UserCode : ("USR" + Guid.NewGuid().ToString("N")[..6].ToUpper()),
                Email = dto.Email ?? "",
                Password = string.IsNullOrWhiteSpace(dto.Password) ? "password123" : dto.Password,
                MobileNumber = dto.MobileNumber ?? "",
                ProfilePicturePath = dto.ProfilePicturePath ?? "",
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _context.Entry(user).Reference(u => u.UserType).LoadAsync();

            return Ok(new ApiResponse<User_Admin_Response_DTO>
            {
                Success = true,
                Message = "User Added Successfully",
                Data = MapToDto(user)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User_Update_DTO dto)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "User not found" });

            if (dto.UserTypeID > 0) user.UserTypeID = dto.UserTypeID;
            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (dto.UserCode != null) user.UserCode = dto.UserCode;
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password)) user.Password = dto.Password;
            if (dto.MobileNumber != null) user.MobileNumber = dto.MobileNumber;
            if (dto.ProfilePicturePath != null) user.ProfilePicturePath = dto.ProfilePicturePath;
            user.IsActive = dto.IsActive;
            user.IsDeleted = dto.IsDeleted;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "User Updated Successfully",
                Data = "Updated"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while updating user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "User not found" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "User Deleted Successfully",
                Data = "Deleted"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while deleting user",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    private static User_Admin_Response_DTO MapToDto(User u) => new()
    {
        UserID = u.UserID,
        UserTypeID = u.UserTypeID,
        UserTypeName = u.UserType?.UserTypeName ?? "",
        FullName = u.FullName ?? "",
        UserCode = u.UserCode,
        Email = u.Email ?? "",
        Password = u.Password ?? "",
        MobileNumber = u.MobileNumber ?? "",
        ProfilePicturePath = u.ProfilePicturePath ?? "",
        IsActive = u.IsActive,
        IsDeleted = u.IsDeleted
    };
}