using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;

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
                UserTypeName = u.UserType.UserTypeName ,
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

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.UserID == id);

        if (user == null)
            return NotFound();

        var response = new User_Admin_Response_DTO
        {
            UserID = user.UserID,
            UserTypeID = user.UserTypeID,
            UserTypeName = user.UserType?.UserTypeName ,
            FullName = user.FullName,
            UserCode = user.UserCode,
            Email = user.Email,
            Password = user.Password,
            MobileNumber = user.MobileNumber,
            ProfilePicturePath = user.ProfilePicturePath,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User_Create_DTO dto)
    {
        var user = new User
        {
            UserTypeID = dto.UserTypeID,
            FullName = dto.FullName,
            UserCode = dto.UserCode,
            Email = dto.Email,
            Password = dto.Password,
            MobileNumber = dto.MobileNumber,
            ProfilePicturePath = dto.ProfilePicturePath,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        await _context.Entry(user).Reference(u => u.UserType).LoadAsync();

        var response = new User_Admin_Response_DTO
        {
            UserID = user.UserID,
            UserTypeID = user.UserTypeID,
            UserTypeName = user.UserType?.UserTypeName ,
            FullName = user.FullName,
            UserCode = user.UserCode,
            Email = user.Email,
            Password = user.Password,
            MobileNumber = user.MobileNumber,
            ProfilePicturePath = user.ProfilePicturePath,
            IsActive = user.IsActive,
            IsDeleted = user.IsDeleted
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User_Update_DTO dto)
    {
        var oldUser = await _context.Users.FindAsync(id);

        if (oldUser == null)
            return NotFound();

        oldUser.UserTypeID = dto.UserTypeID;
        oldUser.FullName = dto.FullName;
        oldUser.UserCode = dto.UserCode;
        oldUser.Email = dto.Email;
        oldUser.Password = dto.Password;
        oldUser.MobileNumber = dto.MobileNumber;
        oldUser.ProfilePicturePath = dto.ProfilePicturePath;
        oldUser.IsActive = dto.IsActive;
        oldUser.IsDeleted = dto.IsDeleted;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok();
    }
}