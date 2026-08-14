using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using SPM.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // 1) Display the total number of students registered in the system.
        [HttpGet("total-students")]
        public async Task<IActionResult> GetTotalStudents()
        {
            var count = await _context.Users
                .CountAsync(u => u.UserType.UserTypeName == "Student" && u.IsDeleted != true);
            return Ok(new { TotalStudents = count });
        }

        // 2) Display the total number of faculty members guiding projects.
        [HttpGet("total-faculties-guiding")]
        public async Task<IActionResult> GetTotalFacultiesGuiding()
        {
            var count = await _context.ProjectAllocations
                .Select(pa => pa.FacultyID)
                .Distinct()
                .CountAsync();
            return Ok(new { TotalFacultiesGuiding = count });
        }

        // 3) Display the total number of projects available in the system.
        [HttpGet("total-projects")]
        public async Task<IActionResult> GetTotalProjects()
        {
            var count = await _context.ProjectMasters.CountAsync();
            return Ok(new { TotalProjects = count });
        }

        // 4) Show how many tasks belong to each status category.
        [HttpGet("tasks-by-status")]
        public async Task<IActionResult> GetTasksByStatus()
        {
            var result = await _context.Tasks
                .GroupBy(t => t.TaskStatus.TaskStatusName)
                .Select(g => new
                {
                    Status = g.Key ?? "Unknown",
                    Tasks = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 5) Show priority wise task count
        [HttpGet("tasks-by-priority")]
        public async Task<IActionResult> GetTasksByPriority()
        {
            var result = await _context.Tasks
                .GroupBy(t => t.TaskPriority.TaskPriorityName)
                .Select(g => new
                {
                    Priority = g.Key ?? "Unknown",
                    Tasks = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 6) Show how many projects are assigned to each faculty member.
        [HttpGet("projects-by-faculty")]
        public async Task<IActionResult> GetProjectsByFaculty()
        {
            var result = await _context.ProjectAllocations
                .GroupBy(pa => pa.Faculty.FullName)
                .Select(g => new
                {
                    Faculty = g.Key,
                    Projects = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 7) Show how many tasks have been assigned to each student.
        [HttpGet("tasks-by-student")]
        public async Task<IActionResult> GetTasksByStudent()
        {
            var result = await _context.Tasks
                .GroupBy(t => t.ProjectAllocation.Student.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    Tasks = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 8) Display the top 10 students having the highest average earned score.
        [HttpGet("top-students")]
        public async Task<IActionResult> GetTopStudents()
        {
            var rawResult = await _context.Tasks
                .Where(t => t.EarnedScore != null)
                .GroupBy(t => t.ProjectAllocation.Student.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    AvgScore = g.Average(t => t.EarnedScore)
                })
                .OrderByDescending(x => x.AvgScore)
                .Take(10)
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Student,
                AvgScore = x.AvgScore.HasValue ? Math.Round(x.AvgScore.Value, 2) : 0
            }).ToList();

            return Ok(result);
        }

        // 9) Display the bottom 10 students based on average earned score.
        [HttpGet("bottom-students")]
        public async Task<IActionResult> GetBottomStudents()
        {
            var rawResult = await _context.Tasks
                .Where(t => t.EarnedScore != null)
                .GroupBy(t => t.ProjectAllocation.Student.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    TotalTasks = g.Count(),
                    AverageScore = g.Average(t => t.EarnedScore)
                })
                .OrderBy(x => x.AverageScore)
                .Take(10)
                .ToListAsync();

            var result = rawResult.Select((x, index) => new
            {
                Rank = index + 1,
                x.Student,
                x.TotalTasks,
                AverageScore = x.AverageScore.HasValue ? Math.Round(x.AverageScore.Value, 2) : 0
            }).ToList();

            return Ok(result);
        }

        // 10) Display all tasks whose due date has passed but are not completed.
        [HttpGet("overdue-tasks")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            var today = DateTime.Today;
            var rawResult = await _context.Tasks
                .Where(t => t.TaskStatus.TaskStatusName != "Completed" && t.TaskDueDate < today)
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskTitle,
                    Student = t.ProjectAllocation.Student.FullName,
                    Faculty = t.ProjectAllocation.Faculty.FullName,
                    DueDate = t.TaskDueDate
                })
                .ToListAsync();

            var result = rawResult.Select(t => new
            {
                TaskID = t.TaskID,
                TaskTitle = t.TaskTitle,
                t.Student,
                t.Faculty,
                DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("dd-MMM-yyyy") : "",
                DaysOverdue = t.DueDate.HasValue ? (today - t.DueDate.Value).Days : 0
            }).ToList();

            return Ok(result);
        }

        // 11) Display tasks having follow-up dates within the next 7 days.
        [HttpGet("upcoming-followups")]
        public async Task<IActionResult> GetUpcomingFollowups()
        {
            var today = DateTime.Today;
            var endLimit = today.AddDays(7);
            var rawResult = await _context.Tasks
                .Where(t => t.NextFollowUpDate >= today && t.NextFollowUpDate <= endLimit)
                .Select(t => new
                {
                    t.TaskTitle,
                    Student = t.ProjectAllocation.Student.FullName,
                    Faculty = t.ProjectAllocation.Faculty.FullName,
                    FollowUpDate = t.NextFollowUpDate
                })
                .ToListAsync();

            var result = rawResult.Select(t => new
            {
                t.TaskTitle,
                t.Student,
                t.Faculty,
                FollowUpDate = t.FollowUpDate.HasValue ? t.FollowUpDate.Value.ToString("dd-MMM-yyyy") : ""
            }).ToList();

            return Ok(result);
        }

        // 12) Show how many students have obtained each grade.
        [HttpGet("grade-distribution")]
        public async Task<IActionResult> GetGradeDistribution()
        {
            var result = await _context.ProjectAllocations
                .Where(pa => pa.OverAllGrade != null && pa.OverAllGrade != "")
                .GroupBy(pa => pa.OverAllGrade)
                .Select(g => new
                {
                    Grade = g.Key,
                    Students = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 13) Show month-wise completed task count (Month names).
        [HttpGet("monthwise-completed-tasks")]
        public async Task<IActionResult> GetMonthwiseCompletedTasks()
        {
            var rawResult = await _context.Tasks
                .Where(t => t.TaskStatus.TaskStatusName == "Completed" && t.TaskCompletedDate != null)
                .GroupBy(t => new { Year = t.TaskCompletedDate.Value.Year, Month = t.TaskCompletedDate.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    CompletedTasks = g.Count()
                })
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Year,
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMMM"),
                x.CompletedTasks
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => DateTime.ParseExact(x.Month, "MMMM", CultureInfo.InvariantCulture).Month)
            .ToList();

            return Ok(result);
        }

        // 14) Display Role Wise Active User Count.
        [HttpGet("active-users-by-role")]
        public async Task<IActionResult> GetActiveUsersByRole()
        {
            var result = await _context.Users
                .Where(u => u.IsActive && u.IsDeleted != true)
                .GroupBy(u => u.UserType.UserTypeName)
                .Select(g => new
                {
                    Role = g.Key,
                    ActiveUsers = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 15) Display each role with users assigned to it.
        [HttpGet("users-by-role")]
        public async Task<IActionResult> GetUsersByRole()
        {
            var result = await _context.Users
                .Where(u => u.IsDeleted != true)
                .OrderBy(u => u.UserType.UserTypeName)
                .ThenBy(u => u.FullName)
                .Select(u => new
                {
                    Role = u.UserType.UserTypeName,
                    UserName = u.FullName
                })
                .ToListAsync();
            return Ok(result);
        }

        // 16) List Roles Having More Than 10 Users.
        [HttpGet("roles-large-user-count")]
        public async Task<IActionResult> GetRolesLargeUserCount()
        {
            var result = await _context.Users
                .Where(u => u.IsDeleted != true)
                .GroupBy(u => u.UserType.UserTypeName)
                .Where(g => g.Count() > 10)
                .Select(g => new
                {
                    Role = g.Key,
                    TotalUsers = g.Count()
                })
                .ToListAsync();
            return Ok(result);
        }

        // 17) Display role statistics.
        [HttpGet("role-statistics")]
        public async Task<IActionResult> GetRoleStatistics()
        {
            var result = await _context.Users
                .Where(u => u.IsDeleted != true)
                .GroupBy(u => u.UserType.UserTypeName)
                .Select(g => new
                {
                    Role = g.Key,
                    TotalUsers = g.Count(),
                    ActiveUsers = g.Count(u => u.IsActive),
                    InactiveUsers = g.Count(u => !u.IsActive)
                })
                .ToListAsync();
            return Ok(result);
        }

        // 18) Show tasks due within next 7 days.
        [HttpGet("upcoming-due-tasks")]
        public async Task<IActionResult> GetUpcomingDueTasks()
        {
            var today = DateTime.Today;
            var endLimit = today.AddDays(7);
            var rawResult = await _context.Tasks
                .Where(t => t.TaskDueDate >= today && t.TaskDueDate <= endLimit)
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskTitle,
                    Project = t.ProjectAllocation.Project.ProjectTitle,
                    Student = t.ProjectAllocation.Student.FullName,
                    DueDate = t.TaskDueDate
                })
                .ToListAsync();

            var result = rawResult.Select(t => new
            {
                t.TaskID,
                t.TaskTitle,
                t.Project,
                t.Student,
                DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("dd-MMM-yyyy") : "",
                DaysRemaining = t.DueDate.HasValue ? (t.DueDate.Value - today).Days : 0
            }).ToList();

            return Ok(result);
        }

        // 19) Display each project with total tasks, completed tasks, pending tasks, and average task progress.
        [HttpGet("project-task-summary")]
        public async Task<IActionResult> GetProjectTaskSummary()
        {
            var rawResult = await _context.ProjectMasters
                .Select(pm => new
                {
                    Project = pm.ProjectTitle,
                    Tasks = _context.Tasks.Count(t => t.ProjectAllocation.ProjectID == pm.ProjectID),
                    Completed = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName == "Completed" && t.ProjectAllocation.ProjectID == pm.ProjectID),
                    Pending = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName != "Completed" && t.ProjectAllocation.ProjectID == pm.ProjectID),
                    AvgProgress = _context.Tasks.Where(t => t.ProjectAllocation.ProjectID == pm.ProjectID).Select(t => (decimal?)t.ProgressPercentage).Average() ?? 0
                })
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Project,
                x.Tasks,
                x.Completed,
                x.Pending,
                AvgProgress = $"{Math.Round(x.AvgProgress, 0)}%"
            }).ToList();

            return Ok(result);
        }

        // 20) Display project-wise total assigned score, earned score, and score percentage.
        [HttpGet("project-score-percentage")]
        public async Task<IActionResult> GetProjectScorePercentage()
        {
            var rawResult = await _context.ProjectMasters
                .Select(pm => new
                {
                    Project = pm.ProjectTitle,
                    TotalAssignedScore = _context.Tasks.Where(t => t.ProjectAllocation.ProjectID == pm.ProjectID).Sum(t => (decimal?)t.AssignedScore) ?? 0,
                    TotalEarnedScore = _context.Tasks.Where(t => t.ProjectAllocation.ProjectID == pm.ProjectID).Sum(t => t.EarnedScore) ?? 0
                })
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Project,
                TotalAssignedScore = x.TotalAssignedScore,
                TotalEarnedScore = x.TotalEarnedScore,
                ScorePercentage = x.TotalAssignedScore > 0 ? $"{(x.TotalEarnedScore / x.TotalAssignedScore * 100):F2}%" : "0.00%"
            }).ToList();

            return Ok(result);
        }

        // 21) Display Top 10 projects based on average earned score.
        [HttpGet("top-projects")]
        public async Task<IActionResult> GetTopProjects()
        {
            var rawResult = await _context.Tasks
                .Where(t => t.EarnedScore != null)
                .GroupBy(t => t.ProjectAllocation.Project.ProjectTitle)
                .Select(g => new
                {
                    Project = g.Key,
                    AverageScore = g.Average(t => t.EarnedScore)
                })
                .OrderByDescending(x => x.AverageScore)
                .Take(10)
                .ToListAsync();

            var result = rawResult.Select((x, index) => new
            {
                Rank = index + 1,
                x.Project,
                AverageScore = x.AverageScore.HasValue ? Math.Round(x.AverageScore.Value, 2) : 0
            }).ToList();

            return Ok(result);
        }

        // 22) Show project count, task count, and average progress for each faculty.
        [HttpGet("faculty-summary")]
        public async Task<IActionResult> GetFacultySummary()
        {
            var rawResult = await _context.ProjectAllocations
                .GroupBy(pa => pa.Faculty.FullName)
                .Select(g => new
                {
                    Faculty = g.Key,
                    TotalProjects = g.Select(pa => pa.ProjectID).Distinct().Count(),
                    TotalTasks = _context.Tasks.Count(t => t.ProjectAllocation.Faculty.FullName == g.Key),
                    AvgProgress = g.Average(pa => pa.ProgressPercentage)
                })
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Faculty,
                x.TotalProjects,
                x.TotalTasks,
                AvgProgress = Math.Round(x.AvgProgress, 2)
            }).ToList();

            return Ok(result);
        }

        // 23) Display task completion statistics and average score for each student.
        [HttpGet("student-completion-stats")]
        public async Task<IActionResult> GetStudentCompletionStats()
        {
            var rawResult = await _context.ProjectAllocations
                .GroupBy(pa => pa.Student.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    TotalTasks = _context.Tasks.Count(t => t.ProjectAllocation.Student.FullName == g.Key),
                    CompletedTasks = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName == "Completed" && t.ProjectAllocation.Student.FullName == g.Key),
                    PendingTasks = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName != "Completed" && t.ProjectAllocation.Student.FullName == g.Key),
                    AvgScore = _context.Tasks.Where(t => t.EarnedScore != null && t.ProjectAllocation.Student.FullName == g.Key).Average(t => (decimal?)t.EarnedScore) ?? 0
                })
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Student,
                x.TotalTasks,
                x.CompletedTasks,
                x.PendingTasks,
                AvgScore = Math.Round(x.AvgScore, 2)
            }).ToList();

            return Ok(result);
        }

        // 24) Display projects whose expected completion date has passed but are still incomplete.
        [HttpGet("overdue-incomplete-projects")]
        public async Task<IActionResult> GetOverdueIncompleteProjects()
        {
            var today = DateTime.Today;
            var rawResult = await _context.ProjectAllocations
                .Where(pa => pa.ProgressPercentage < 100 && pa.ProjectEndDate < today)
                .Select(pa => new
                {
                    Project = pa.Project.ProjectTitle,
                    Student = pa.Student.FullName,
                    Faculty = pa.Faculty.FullName,
                    EndDate = pa.ProjectEndDate,
                    Progress = pa.ProgressPercentage
                })
                .ToListAsync();

            var result = rawResult.Select(x => new
            {
                x.Project,
                x.Student,
                x.Faculty,
                EndDate = x.EndDate.ToString("dd-MMM-yyyy"),
                Progress = $"{Math.Round(x.Progress, 0)}%"
            }).ToList();

            return Ok(result);
        }

        // 25) Show month-wise completed task count (Month as integer).
        [HttpGet("monthwise-completed-tasks-int")]
        public async Task<IActionResult> GetMonthwiseCompletedTasksInt()
        {
            var result = await _context.Tasks
                .Where(t => t.TaskStatus.TaskStatusName == "Completed" && t.TaskCompletedDate != null)
                .GroupBy(t => new { Year = t.TaskCompletedDate.Value.Year, Month = t.TaskCompletedDate.Value.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    CompletedTasks = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return Ok(result);
        }

        // 26) Rank faculties based on average project progress.
        [HttpGet("faculty-progress-ranking")]
        public async Task<IActionResult> GetFacultyProgressRanking()
        {
            var rawResult = await _context.ProjectAllocations
                .GroupBy(pa => pa.Faculty.FullName)
                .Select(g => new
                {
                    Faculty = g.Key,
                    AvgProgress = g.Average(pa => pa.ProgressPercentage)
                })
                .OrderByDescending(x => x.AvgProgress)
                .ToListAsync();

            var result = rawResult.Select((x, index) => new
            {
                Rank = index + 1,
                x.Faculty,
                AvgProgress = Math.Round(x.AvgProgress, 2)
            }).ToList();

            return Ok(result);
        }

        // 27) Display task statistics for every project.
        [HttpGet("project-task-details")]
        public async Task<IActionResult> GetProjectTaskDetails()
        {
            var today = DateTime.Today;
            var result = await _context.ProjectMasters
                .Select(pm => new
                {
                    Project = pm.ProjectTitle,
                    TotalTasks = _context.Tasks.Count(t => t.ProjectAllocation.ProjectID == pm.ProjectID),
                    Completed = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName == "Completed" && t.ProjectAllocation.ProjectID == pm.ProjectID),
                    Pending = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName != "Completed" && t.ProjectAllocation.ProjectID == pm.ProjectID),
                    Overdue = _context.Tasks.Count(t => t.TaskStatus.TaskStatusName != "Completed" && t.TaskDueDate < today && t.ProjectAllocation.ProjectID == pm.ProjectID)
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}