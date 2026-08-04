using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace SPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserRoleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/UserRole
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRole>>> GetUserRoles()
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        // GET: api/UserRole/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRole>> GetUserRole(int id)
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => ur.RolePermissionID == id);

            if (userRole == null)
            {
                return NotFound("User Role not found.");
            }

            return Ok(userRole);
        }

        // POST: api/UserRole
        [HttpPost]
        public async Task<ActionResult<UserRole>> AddUserRole(UserRole userRole)
        {
            userRole.RolePermissionID = 0;
            // Check Role
            var roleExists = await _context.Roles.AnyAsync(r => r.RoleID == userRole.RoleID);
            if (!roleExists)
            {
                return BadRequest("Invalid Role ID.");
            }

            // Check User
            var userExists = await _context.Users.AnyAsync(u => u.UserID == userRole.UserID);
            if (!userExists)
            {
                return BadRequest("Invalid User ID.");
            }

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserRole),
                new { id = userRole.RolePermissionID }, userRole);
        }

        // PUT: api/UserRole/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserRole(int id, UserRole userRole)
        {
            if (id != userRole.RolePermissionID)
            {
                return BadRequest("ID mismatch.");
            }

            var existingUserRole = await _context.UserRoles.FindAsync(id);

            if (existingUserRole == null)
            {
                return NotFound("User Role not found.");
            }

            existingUserRole.RoleID = userRole.RoleID;
            existingUserRole.UserID = userRole.UserID;

            await _context.SaveChangesAsync();

            return Ok("User Role updated successfully.");
        }

        // DELETE: api/UserRole/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserRole(int id)
        {
            var userRole = await _context.UserRoles.FindAsync(id);

            if (userRole == null)
            {
                return NotFound("User Role not found.");
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return Ok("User Role deleted successfully.");
        }
    }
}