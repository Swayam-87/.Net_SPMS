using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace SPM.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserType)
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User user)
        {
            user.UserID = 0;
            // Check UserType
            var userType = await _context.UserTypes.FindAsync(user.UserTypeID);

            if (userType == null)
            {
                return BadRequest("Invalid UserTypeID.");
            }

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);

            if (emailExists)
            {
                return BadRequest("Email already exists.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser),
                new { id = user.UserID }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            if (id != user.UserID)
            {
                return BadRequest("User ID mismatch.");
            }

            var existingUser = await _context.Users.FindAsync(id);

            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            await _context.SaveChangesAsync();

            return Ok("User updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }
    }
}