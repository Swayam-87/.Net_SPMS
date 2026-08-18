using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

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
        try
        {
            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.Password == dto.Password);

            if (user == null)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid email or password."
                });

            if (!user.IsActive)
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Account is inactive. Please contact admin."
                });

            var response = new Login_Response_DTO
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                UserTypeName = user.UserType?.UserTypeName ?? "",
                ProfilePicturePath = user.ProfilePicturePath
            };

            return Ok(new ApiResponse<Login_Response_DTO>
            {
                Success = true,
                Message = "Login Successful",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred during login",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
