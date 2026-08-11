using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoleController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles
            .Select(r => new Role_Admin_Response_DTO
            {
                RoleID = r.RoleID,
                RoleName = r.RoleName,
                Description = r.Description
            })
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return NotFound();

        var response = new Role_Admin_Response_DTO
        {
            RoleID = role.RoleID,
            RoleName = role.RoleName,
            Description = role.Description
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Role_Create_DTO dto)
    {
        var role = new Role
        {
            RoleName = dto.RoleName,
            Description = dto.Description
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        var response = new Role_Admin_Response_DTO
        {
            RoleID = role.RoleID,
            RoleName = role.RoleName,
            Description = role.Description
        };

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Role_Update_DTO dto)
    {
        var oldRole = await _context.Roles.FindAsync(id);

        if (oldRole == null)
            return NotFound();

        oldRole.RoleName = dto.RoleName;
        oldRole.Description = dto.Description;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = await _context.Roles.FindAsync(id);

        if (role == null)
            return NotFound();

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();

        return Ok();
    }
}