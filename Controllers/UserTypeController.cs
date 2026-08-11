using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;

[ApiController]
[Route("api/[controller]")]
public class UserTypeController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserTypeController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
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

        return Ok(userTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserType(int id)
    {
        var userType = await _context.UserTypes.FindAsync(id);

        if (userType == null)
            return NotFound();

        var response = new UserType_Admin_Response_DTO
        {
            UserTypeID = userType.UserTypeID,
            UserTypeName = userType.UserTypeName,
            Description = userType.Description
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserType_Create_DTO dto)
    {
        var userType = new UserType
        {
            UserTypeName = dto.UserTypeName,
            Description = dto.Description
        };

        _context.UserTypes.Add(userType);
        await _context.SaveChangesAsync();

        var response = new UserType_Admin_Response_DTO
        {
            UserTypeID = userType.UserTypeID,
            UserTypeName = userType.UserTypeName,
            Description = userType.Description
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserType_Update_DTO dto)
    {
        var oldUserType = await _context.UserTypes.FindAsync(id);

        if (oldUserType == null)
            return NotFound();

        oldUserType.UserTypeName = dto.UserTypeName;
        oldUserType.Description = dto.Description;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userType = await _context.UserTypes.FindAsync(id);

        if (userType == null)
            return NotFound();

        _context.UserTypes.Remove(userType);
        await _context.SaveChangesAsync();

        return Ok();
    }
}