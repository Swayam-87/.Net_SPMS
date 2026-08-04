using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

namespace StudentProManagement.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class SPM_TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SPM_TaskController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var data = await _context.Tasks.ToListAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var data = await _context.Tasks.FindAsync(id);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> Post(SPM_Task model)
        {
            model.TaskID = 0;
            _context.Tasks.Add(model);
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, SPM_Task model)
        {
            if (id != model.TaskID) return BadRequest();
            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var data = await _context.Tasks.FindAsync(id);
            if (data == null) return NotFound();
            _context.Tasks.Remove(data);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted Successfully" });
        }
    }
}
