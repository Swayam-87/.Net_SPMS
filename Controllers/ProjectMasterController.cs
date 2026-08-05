using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectMasterController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectMasterController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _context.ProjectMasters.ToListAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var project = await _context.ProjectMasters.FindAsync(id);

        if (project == null)
            return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectMaster project)
    {
        _context.ProjectMasters.Add(project);
        await _context.SaveChangesAsync();

        return Ok(project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProjectMaster project)
    {
        if (id != project.ProjectID)
            return BadRequest();

        var oldProject = await _context.ProjectMasters.FindAsync(id);

        oldProject.ProjectTitle = project.ProjectTitle;
        oldProject.Description = project.Description;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.ProjectMasters.FindAsync(id);

        if (project == null)
            return NotFound();

        _context.ProjectMasters.Remove(project);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
