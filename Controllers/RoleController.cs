using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace SPM.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoleController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound("Role not found.");
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<Role>> AddRole(Role role)
        {
            role.RoleID = 0;
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

           return Ok(role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, Role role)
        {
            if (id != role.RoleID)
            {
                return BadRequest("Role ID mismatch.");
            }

            var existingRole = await _context.Roles.FindAsync(id);

            if (existingRole == null)
            {
                return NotFound("Role not found.");
            }

            existingRole.RoleName = role.RoleName;
            existingRole.Description = role.Description;

            await _context.SaveChangesAsync();

            return Ok("Role updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound("Role not found.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok("Role deleted successfully.");
        }
    }
}