using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace SPM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserType>>> Get()
        {
            return await _context.UserTypes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserType>> Get(int id)
        {
            var data = await _context.UserTypes.FindAsync(id);

            if (data == null)
                return NotFound();

            return data;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserType userType)
        {
            userType.UserTypeID = 0;
            _context.UserTypes.Add(userType);
            await _context.SaveChangesAsync();
            return Ok(userType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UserType userType)
        {
            if (id != userType.UserTypeID)
                return BadRequest();

            _context.Entry(userType).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var data = await _context.UserTypes.FindAsync(id);

            if (data == null)
                return NotFound();

            _context.UserTypes.Remove(data);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}