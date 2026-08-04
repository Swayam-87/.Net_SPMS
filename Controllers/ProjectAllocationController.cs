using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace StudentProManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAllocationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectAllocationController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var data = await _context.ProjectAllocations.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var data = await _context.ProjectAllocations.FindAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ProjectAllocation model)
        {
            model.ProjectAllocationID = 0;
            _context.ProjectAllocations.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, ProjectAllocation model)
        {
            if (id != model.ProjectAllocationID) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var data = await _context.ProjectAllocations.FindAsync(id);
            if (data == null) return NotFound();
            _context.ProjectAllocations.Remove(data);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted Successfully" });
        }
    }
}
