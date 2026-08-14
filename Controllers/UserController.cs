using Microsoft.AspNetCore.Http.HttpResults;
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
            .Include(u => u.UserType)
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
            Data = users,
        });
    }

    [HttpGet("{id}")]
    [HttpGet("GetUserById/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return NotFound(new ApiResponse<User_Admin_Response_DTO> { Success = false, Message = "User not found" });

        var response = new User_Admin_Response_DTO
        {
            UserID = user.UserID,
            UserTypeID = user.UserTypeID,
            UserTypeName = user.UserType?.UserTypeName,
            FullName = user.FullName,
            UserCode = user.UserCode,
            Email = user.Email,
            Password = user.Password,
            MobileNumber = user.MobileNumber,
            ProfilePicturePath = user.ProfilePicturePath,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted
        };

        return Ok(new ApiResponse<User_Admin_Response_DTO>
        {
            Success = true,
            Message = "User Retrieved Successfully",
            Data = response
        });
    }

    [HttpPost]
    [HttpPost("CreateUser")]
    public async Task<IActionResult> Create(User_Create_DTO dto)
    {
        if (dto == null)
            return BadRequest(new ApiResponse<User_Admin_Response_DTO> { Success = false, Message = "Invalid user payload" });

        // Ensure foreign key UserTypeID exists in the database
        var targetType = await _context.UserTypes.FirstOrDefaultAsync(ut => ut.UserTypeID == dto.UserTypeID);
        if (targetType == null)
        {
            targetType = await _context.UserTypes.FirstOrDefaultAsync();
            if (targetType == null)
            {
                var defaultTypes = new List<UserType>
                {
                    new UserType { UserTypeName = "Admin", Description = "System Administrator" },
                    new UserType { UserTypeName = "Faculty", Description = "Faculty Staff" },
                    new UserType { UserTypeName = "Student", Description = "Student User" }
                };
                _context.UserTypes.AddRange(defaultTypes);
                await _context.SaveChangesAsync();
                targetType = defaultTypes.FirstOrDefault(ut => ut.UserTypeID == dto.UserTypeID) ?? defaultTypes.First();
            }
        }

        var user = new User
        {
            UserTypeID = targetType.UserTypeID,
            FullName = dto.FullName ?? "",
            UserCode = !string.IsNullOrWhiteSpace(dto.UserCode) ? dto.UserCode : ("USR" + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()),
            Email = dto.Email ?? "",
            Password = string.IsNullOrWhiteSpace(dto.Password) ? "password123" : dto.Password,
            MobileNumber = dto.MobileNumber ?? "",
            ProfilePicturePath = dto.ProfilePicturePath ?? "",
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _context.Entry(user).Reference(u => u.UserType).LoadAsync();

        var response = new User_Admin_Response_DTO
        {
            UserID = user.UserID,
            UserTypeID = user.UserTypeID,
            UserTypeName = user.UserType?.UserTypeName,
            FullName = user.FullName,
            UserCode = user.UserCode,
            Email = user.Email,
            Password = user.Password,
            MobileNumber = user.MobileNumber,
            ProfilePicturePath = user.ProfilePicturePath,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted
        };

        return Ok(new ApiResponse<User_Admin_Response_DTO>
        {
            Success = true,
            Message = "User Created Successfully",
            Data = response
        });
    }

    [HttpPut("{id}")]
    [HttpPut("UpdateUser/{id}")]
    public async Task<IActionResult> Update(int id, User_Update_DTO dto)
    {
        var oldUser = await _context.Users.FindAsync(id);

        if (oldUser == null)
            return NotFound(new ApiResponse<string> { Success = false, Message = "User not found" });

        if (dto.UserTypeID > 0) oldUser.UserTypeID = dto.UserTypeID;
        if (!string.IsNullOrEmpty(dto.FullName)) oldUser.FullName = dto.FullName;
        if (dto.UserCode != null) oldUser.UserCode = dto.UserCode;
        if (!string.IsNullOrEmpty(dto.Email)) oldUser.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.Password)) oldUser.Password = dto.Password;
        if (dto.MobileNumber != null) oldUser.MobileNumber = dto.MobileNumber;
        if (dto.ProfilePicturePath != null) oldUser.ProfilePicturePath = dto.ProfilePicturePath;
        oldUser.IsActive = dto.IsActive;
        oldUser.IsDeleted = dto.IsDeleted;

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "User Updated Successfully",
            Data = "Updated"
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
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
}