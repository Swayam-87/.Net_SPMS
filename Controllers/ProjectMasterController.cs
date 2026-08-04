using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace StudentProManagement.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectMasterController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectMasterController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var data = await _context.ProjectMasters.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var data = await _context.ProjectMasters.FindAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> Post(ProjectMaster model)
        {
            model.ProjectID = 0;
            _context.ProjectMasters.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, ProjectMaster model)
        {
            if (id != model.ProjectID) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var data = await _context.ProjectMasters.FindAsync(id);
            if (data == null) return NotFound();
            _context.ProjectMasters.Remove(data);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted Successfully" });
        }
    }
}
