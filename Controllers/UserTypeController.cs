using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

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
        var userTypes = await _context.UserTypes.ToListAsync();
        return Ok(userTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserType(int id)
    {
        var userType = await _context.UserTypes.FindAsync(id);

        if (userType == null)
            return NotFound();

        return Ok(userType);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserType userType)
    {
        _context.UserTypes.Add(userType);
        await _context.SaveChangesAsync();

        return Ok(userType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserType userType)
    {
        if (id != userType.UserTypeID)
            return BadRequest();

        var oldUserType = await _context.UserTypes.FindAsync(id);

        oldUserType.UserTypeName = userType.UserTypeName;
        oldUserType.Description = userType.Description;
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