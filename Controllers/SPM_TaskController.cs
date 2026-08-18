using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using StudentProManagement.DTO_s;
using StudentProManagement.Models;

[ApiController]
[Route("api/[controller]")]
public class SPM_TaskController : ControllerBase
{
    private readonly AppDbContext _context;

    public SPM_TaskController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _context.Tasks
            .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.Project)
            .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.Student)
            .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.Faculty)
            .Include(t => t.TaskStatus)
            .Include(t => t.TaskPriority)
            .Select(t => new SPM_Task_Admin_Response_DTO
            {
                TaskID = t.TaskID,
                ProjectAllocationID = t.ProjectAllocationID,
                ProjectTitle = t.ProjectAllocation.Project.ProjectTitle ?? "",
                StudentName = t.ProjectAllocation.Student.FullName ?? "",
                FacultyName = t.ProjectAllocation.Faculty.FullName ?? "",
                TaskTitle = t.TaskTitle,
                TaskDescription = t.TaskDescription,
                TaskStatusID = t.TaskStatusID,
                TaskStatusName = t.TaskStatus.TaskStatusName ?? "",
                TaskPriorityID = t.TaskPriorityID,
                TaskPriorityName = t.TaskPriority.TaskPriorityName ?? "",
                AssignedScore = t.AssignedScore,
                EarnedScore = t.EarnedScore,
                ProgressPercentage = t.ProgressPercentage,
                TaskAssignedDate = t.TaskAssignedDate,
                TaskStartDate = t.TaskStartDate,
                TaskDueDate = t.TaskDueDate,
                TaskCompletedDate = t.TaskCompletedDate,
                NextFollowUpDate = t.NextFollowUpDate,
                FacultyRemarks = t.FacultyRemarks,
                StudentRemarks = t.StudentRemarks
            })
            .ToListAsync();

        return Ok(new ApiResponse<List<SPM_Task_Admin_Response_DTO>>
        {
            Success = true,
            Message = "Tasks Retrieved Successfully",
            Data = tasks,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.Project)
            .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.Student)
            .Include(t => t.ProjectAllocation)
                .ThenInclude(pa => pa.Faculty)
            .Include(t => t.TaskStatus)
            .Include(t => t.TaskPriority)
            .FirstOrDefaultAsync(t => t.TaskID == id);

        if (task == null)
            return NotFound();

        var response = new SPM_Task_Admin_Response_DTO
        {
            TaskID = task.TaskID,
            ProjectAllocationID = task.ProjectAllocationID,
            ProjectTitle = task.ProjectAllocation?.Project?.ProjectTitle ?? "",
            StudentName = task.ProjectAllocation?.Student?.FullName ?? "",
            FacultyName = task.ProjectAllocation?.Faculty?.FullName ?? "",
            TaskTitle = task.TaskTitle,
            TaskDescription = task.TaskDescription,
            TaskStatusID = task.TaskStatusID,
            TaskStatusName = task.TaskStatus?.TaskStatusName ?? "",
            TaskPriorityID = task.TaskPriorityID,
            TaskPriorityName = task.TaskPriority?.TaskPriorityName ?? "",
            AssignedScore = task.AssignedScore,
            EarnedScore = task.EarnedScore,
            ProgressPercentage = task.ProgressPercentage,
            TaskAssignedDate = task.TaskAssignedDate,
            TaskStartDate = task.TaskStartDate,
            TaskDueDate = task.TaskDueDate,
            TaskCompletedDate = task.TaskCompletedDate,
            NextFollowUpDate = task.NextFollowUpDate,
            FacultyRemarks = task.FacultyRemarks,
            StudentRemarks = task.StudentRemarks
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SPM_Task_Create_DTO dto)
    {
        try
        {
            var task = new SPM_Task
            {
                ProjectAllocationID = dto.ProjectAllocationID,
                TaskTitle = dto.TaskTitle,
                TaskDescription = dto.TaskDescription,
                TaskStatusID = dto.TaskStatusID,
                TaskPriorityID = dto.TaskPriorityID,
                AssignedScore = dto.AssignedScore,
                TaskAssignedDate = dto.TaskAssignedDate,
                TaskDueDate = dto.TaskDueDate
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Reload with navigation properties for response
            await _context.Entry(task).Reference(t => t.ProjectAllocation).LoadAsync();
            if (task.ProjectAllocation != null)
            {
                await _context.Entry(task.ProjectAllocation).Reference(pa => pa.Project).LoadAsync();
                await _context.Entry(task.ProjectAllocation).Reference(pa => pa.Student).LoadAsync();
                await _context.Entry(task.ProjectAllocation).Reference(pa => pa.Faculty).LoadAsync();
            }
            await _context.Entry(task).Reference(t => t.TaskStatus).LoadAsync();
            await _context.Entry(task).Reference(t => t.TaskPriority).LoadAsync();

            var response = new SPM_Task_Admin_Response_DTO
            {
                TaskID = task.TaskID,
                ProjectAllocationID = task.ProjectAllocationID,
                ProjectTitle = task.ProjectAllocation?.Project?.ProjectTitle ?? "",
                StudentName = task.ProjectAllocation?.Student?.FullName ?? "",
                FacultyName = task.ProjectAllocation?.Faculty?.FullName ?? "",
                TaskTitle = task.TaskTitle,
                TaskDescription = task.TaskDescription,
                TaskStatusID = task.TaskStatusID,
                TaskStatusName = task.TaskStatus?.TaskStatusName ?? "",
                TaskPriorityID = task.TaskPriorityID,
                TaskPriorityName = task.TaskPriority?.TaskPriorityName ?? "",
                AssignedScore = task.AssignedScore,
                EarnedScore = task.EarnedScore,
                ProgressPercentage = task.ProgressPercentage,
                TaskAssignedDate = task.TaskAssignedDate,
                TaskStartDate = task.TaskStartDate,
                TaskDueDate = task.TaskDueDate,
                TaskCompletedDate = task.TaskCompletedDate,
                NextFollowUpDate = task.NextFollowUpDate,
                FacultyRemarks = task.FacultyRemarks,
                StudentRemarks = task.StudentRemarks
            };

            return Ok(new ApiResponse<SPM_Task_Admin_Response_DTO>
            {
                Success = true,
                Message = "Task Added Successfully",
                Data = response
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Error occurred while adding task",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SPM_Task_Update_DTO dto)
    {
        var oldTask = await _context.Tasks.FindAsync(id);

        if (oldTask == null)
            return NotFound();

        oldTask.ProjectAllocationID = dto.ProjectAllocationID;
        oldTask.TaskTitle = dto.TaskTitle;
        oldTask.TaskDescription = dto.TaskDescription;
        oldTask.TaskStatusID = dto.TaskStatusID;
        oldTask.TaskPriorityID = dto.TaskPriorityID;
        oldTask.AssignedScore = dto.AssignedScore;
        oldTask.EarnedScore = dto.EarnedScore;
        oldTask.ProgressPercentage = dto.ProgressPercentage;
        oldTask.TaskAssignedDate = dto.TaskAssignedDate;
        oldTask.TaskStartDate = dto.TaskStartDate;
        oldTask.TaskDueDate = dto.TaskDueDate;
        oldTask.TaskCompletedDate = dto.TaskCompletedDate;
        oldTask.NextFollowUpDate = dto.NextFollowUpDate;
        oldTask.FacultyRemarks = dto.FacultyRemarks;
        oldTask.StudentRemarks = dto.StudentRemarks;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
            return NotFound();

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
