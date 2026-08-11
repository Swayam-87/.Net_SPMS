using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using StudentProManagement.DTO_s;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;

    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Login(Login_Request_DTO dto)
    {
        var user = await _context.Users
            .Include(u => u.UserType)
            .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid email or password." });

        if (!user.IsActive)
            return Unauthorized(new { message = "Account is inactive. Please contact admin." });

        var response = new Login_Response_DTO
        {
            UserID = user.UserID,
            FullName = user.FullName,
            Email = user.Email,
            UserTypeName = user.UserType?.UserTypeName ?? "",
            ProfilePicturePath = user.ProfilePicturePath
        };

        return Ok(response);
    }
}
