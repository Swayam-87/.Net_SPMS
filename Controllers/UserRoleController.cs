using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

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
        var userRoles = await _context.UserRoles.ToListAsync();
        return Ok(userRoles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserRole(int id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);

        if (userRole == null)
            return NotFound();

        return Ok(userRole);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserRole userRole)
    {
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        return Ok(userRole);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UserRole userRole)
    {
        if (id != userRole.RolePermissionID)
            return BadRequest();

        var oldUserRole = await _context.UserRoles.FindAsync(id);

        oldUserRole.RoleID = userRole.RoleID;
        oldUserRole.UserID = userRole.UserID;
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