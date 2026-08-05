using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectAllocationController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectAllocationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectAllocations()
    {
        var allocations = await _context.ProjectAllocations.ToListAsync();
        return Ok(allocations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectAllocation(int id)
    {
        var allocation = await _context.ProjectAllocations.FindAsync(id);

        if (allocation == null)
            return NotFound();

        return Ok(allocation);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectAllocation allocation)
    {
        _context.ProjectAllocations.Add(allocation);
        await _context.SaveChangesAsync();

        return Ok(allocation);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProjectAllocation allocation)
    {
        if (id != allocation.ProjectAllocationID)
            return BadRequest();

        var oldAllocation = await _context.ProjectAllocations.FindAsync(id);

        oldAllocation.ProjectID = allocation.ProjectID;
        oldAllocation.StudentID = allocation.StudentID;
        oldAllocation.FacultyID = allocation.FacultyID;
        oldAllocation.AssignedDate = allocation.AssignedDate;
        oldAllocation.ProjectStartDate = allocation.ProjectStartDate;
        oldAllocation.ProjectEndDate = allocation.ProjectEndDate;
        oldAllocation.TotalTasksGiven = allocation.TotalTasksGiven;
        oldAllocation.TotalCompletedTasks = allocation.TotalCompletedTasks;
        oldAllocation.ProgressPercentage = allocation.ProgressPercentage;
        oldAllocation.OverAllGrade = allocation.OverAllGrade;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var allocation = await _context.ProjectAllocations.FindAsync(id);

        if (allocation == null)
            return NotFound();

        _context.ProjectAllocations.Remove(allocation);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
