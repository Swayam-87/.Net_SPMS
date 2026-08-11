using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPM.Data;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        #region Existing Role Dashboards

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            return await GetAdminDashboard();
        }

        // GET: api/Dashboard/Admin
        [HttpGet("Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var totalStudents = await _context.Users
                .Include(u => u.UserType)
                .CountAsync(u => u.UserType != null && u.UserType.UserTypeName.ToLower() == "student");

            var totalFaculty = await _context.Users
                .Include(u => u.UserType)
                .CountAsync(u => u.UserType != null && u.UserType.UserTypeName.ToLower() == "faculty");

            var totalProjects = await _context.ProjectMasters.CountAsync();
            var totalTasks = await _context.Tasks.CountAsync();

            var totalCompletedTasks = await _context.Tasks
                .Include(t => t.TaskStatus)
                .CountAsync(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") 
                              || t.ProgressPercentage == 100 
                              || t.TaskCompletedDate != null);

            var dto = new Dashboard_Admin_DTO
            {
                TotalStudents = totalStudents,
                TotalFaculty = totalFaculty,
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                TotalCompletedTasks = totalCompletedTasks,
                TotalPendingTasks = totalTasks >= totalCompletedTasks ? totalTasks - totalCompletedTasks : 0
            };

            return Ok(dto);
        }

        // GET: api/Dashboard/Faculty/{facultyId}
        [HttpGet("Faculty/{facultyId}")]
        public async Task<IActionResult> GetFacultyDashboard(int facultyId)
        {
            var allocations = _context.ProjectAllocations.Where(pa => pa.FacultyID == facultyId);

            var totalAssignedStudents = await allocations
                .Select(pa => pa.StudentID)
                .Distinct()
                .CountAsync();

            var totalProjects = await allocations
                .Select(pa => pa.ProjectID)
                .Distinct()
                .CountAsync();

            var facultyTasks = _context.Tasks
                .Include(t => t.ProjectAllocation)
                .Include(t => t.TaskStatus)
                .Where(t => t.ProjectAllocation != null && t.ProjectAllocation.FacultyID == facultyId);

            var totalTasksGiven = await facultyTasks.CountAsync();

            var totalCompletedTasks = await facultyTasks
                .CountAsync(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") 
                              || t.ProgressPercentage == 100 
                              || t.TaskCompletedDate != null);

            var dto = new Dashboard_Faculty_DTO
            {
                TotalAssignedStudents = totalAssignedStudents,
                TotalProjects = totalProjects,
                TotalTasksGiven = totalTasksGiven,
                TotalCompletedTasks = totalCompletedTasks,
                TotalPendingTasks = totalTasksGiven >= totalCompletedTasks ? totalTasksGiven - totalCompletedTasks : 0
            };

            return Ok(dto);
        }

        // GET: api/Dashboard/Student/{studentId}
        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetStudentDashboard(int studentId)
        {
            var allocations = _context.ProjectAllocations.Where(pa => pa.StudentID == studentId);

            var totalProjects = await allocations
                .Select(pa => pa.ProjectID)
                .Distinct()
                .CountAsync();

            var studentTasks = _context.Tasks
                .Include(t => t.ProjectAllocation)
                .Include(t => t.TaskStatus)
                .Where(t => t.ProjectAllocation != null && t.ProjectAllocation.StudentID == studentId);

            var totalTasks = await studentTasks.CountAsync();

            var totalCompletedTasks = await studentTasks
                .CountAsync(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") 
                              || t.ProgressPercentage == 100 
                              || t.TaskCompletedDate != null);

            var avgProgress = await allocations.AnyAsync() 
                ? await allocations.AverageAsync(pa => pa.ProgressPercentage) 
                : 0m;

            var dto = new Dashboard_Student_DTO
            {
                TotalProjects = totalProjects,
                TotalTasks = totalTasks,
                TotalCompletedTasks = totalCompletedTasks,
                TotalPendingTasks = totalTasks >= totalCompletedTasks ? totalTasks - totalCompletedTasks : 0,
                OverallProgress = Math.Round(avgProgress, 2)
            };

            return Ok(dto);
        }

        #endregion

        #region LINQ Practical Tasks 1 - 27

        // 1) Display the total number of students registered in the system.
        [HttpGet("total-students")]
        public async Task<IActionResult> GetTotalStudents()
        {
            var totalStudents = await _context.Users
                .Include(u => u.UserType)
                .CountAsync(u => u.UserType != null && u.UserType.UserTypeName.ToLower() == "student");

            return Ok(totalStudents);
        }

        // 2) Display the total number of faculty members guiding projects.
        [HttpGet("total-faculty")]
        public async Task<IActionResult> GetTotalFacultyGuidingProjects()
        {
            var totalFaculty = await _context.ProjectAllocations
                .Select(pa => pa.FacultyID)
                .Distinct()
                .CountAsync();

            return Ok(totalFaculty);
        }

        // 3) Display the total number of projects available in the system.
        [HttpGet("total-projects")]
        public async Task<IActionResult> GetTotalProjects()
        {
            var totalProjects = await _context.ProjectMasters.CountAsync();

            return Ok(totalProjects);
        }

        // 4) Show how many tasks belong to each status category.
        [HttpGet("tasks-by-status")]
        public async Task<IActionResult> GetTasksByStatusCategory()
        {
            var result = await _context.Tasks
                .Include(t => t.TaskStatus)
                .GroupBy(t => t.TaskStatus != null ? t.TaskStatus.TaskStatusName : "Unknown")
                .Select(g => new { Status = g.Key, Tasks = g.Count() })
                .ToListAsync();

            return Ok(result);
        }

        // 5) Show priority wise task count
        [HttpGet("tasks-by-priority")]
        public async Task<IActionResult> GetTasksByPriority()
        {
            var result = await _context.Tasks
                .Include(t => t.TaskPriority)
                .GroupBy(t => t.TaskPriority != null ? t.TaskPriority.TaskPriorityName : "Unknown")
                .Select(g => new { Priority = g.Key, Tasks = g.Count() })
                .ToListAsync();

            return Ok(result);
        }

        // 6) Show how many projects are assigned to each faculty member.
        [HttpGet("projects-per-faculty")]
        public async Task<IActionResult> GetProjectsPerFaculty()
        {
            var result = await _context.ProjectAllocations
                .Include(pa => pa.Faculty)
                .Where(pa => pa.Faculty != null)
                .GroupBy(pa => pa.Faculty!.FullName)
                .Select(g => new { Faculty = g.Key, Projects = g.Select(pa => pa.ProjectID).Distinct().Count() })
                .ToListAsync();

            return Ok(result);
        }

        // 7) Show how many tasks have been assigned to each student.
        [HttpGet("tasks-per-student")]
        public async Task<IActionResult> GetTasksPerStudent()
        {
            var result = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Student)
                .Where(t => t.ProjectAllocation != null && t.ProjectAllocation.Student != null)
                .GroupBy(t => t.ProjectAllocation!.Student!.FullName)
                .Select(g => new { Student = g.Key, Tasks = g.Count() })
                .ToListAsync();

            return Ok(result);
        }

        // 8) Display the top 10 students having the highest average earned score.
        [HttpGet("top-10-students-by-score")]
        public async Task<IActionResult> GetTop10StudentsByEarnedScore()
        {
            var result = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Student)
                .Where(t => t.ProjectAllocation != null && t.ProjectAllocation.Student != null && t.EarnedScore.HasValue)
                .GroupBy(t => new { t.ProjectAllocation!.StudentID, t.ProjectAllocation.Student!.FullName })
                .Select(g => new
                {
                    Student = g.Key.FullName,
                    AvgScore = Math.Round(g.Average(t => (double)t.EarnedScore!.Value), 2)
                })
                .OrderByDescending(x => x.AvgScore)
                .Take(10)
                .ToListAsync();

            return Ok(result);
        }

        // 9) Display the bottom 10 students based on average earned score.
        [HttpGet("bottom-10-students-by-score")]
        public async Task<IActionResult> GetBottom10StudentsByScore()
        {
            var bottomStudents = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Student)
                .Where(t => t.ProjectAllocation != null && t.ProjectAllocation.Student != null && t.EarnedScore.HasValue)
                .GroupBy(t => new { t.ProjectAllocation!.StudentID, t.ProjectAllocation.Student!.FullName })
                .Select(g => new
                {
                    Student = g.Key.FullName,
                    TotalTasks = g.Count(),
                    AverageScore = Math.Round(g.Average(t => (double)t.EarnedScore!.Value), 2)
                })
                .OrderBy(x => x.AverageScore)
                .Take(10)
                .ToListAsync();

            var result = bottomStudents.Select((s, index) => new
            {
                Rank = index + 1,
                s.Student,
                s.TotalTasks,
                s.AverageScore
            });

            return Ok(result);
        }

        // 10) Display all tasks whose due date has passed but are not completed.
        [HttpGet("overdue-tasks")]
        public async Task<IActionResult> GetOverdueTasks()
        {
            var today = DateTime.Today;
            var result = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Student)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Faculty)
                .Include(t => t.TaskStatus)
                .Where(t => t.TaskDueDate.HasValue && t.TaskDueDate.Value < today && (t.TaskStatus == null || t.TaskStatus.TaskStatusName.ToLower() != "completed"))
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskTitle,
                    Student = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : "",
                    Faculty = t.ProjectAllocation != null && t.ProjectAllocation.Faculty != null ? t.ProjectAllocation.Faculty.FullName : "",
                    DueDate = t.TaskDueDate!.Value.ToString("dd-MMM-yyyy"),
                    DaysOverdue = (today - t.TaskDueDate.Value.Date).Days
                })
                .ToListAsync();

            return Ok(result);
        }

        // 11) Display tasks having follow-up dates within the next 7 days.
        [HttpGet("followup-tasks-next-7-days")]
        public async Task<IActionResult> GetFollowupTasksNext7Days()
        {
            var today = DateTime.Today;
            var next7Days = today.AddDays(7);
            var result = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Student)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Faculty)
                .Where(t => t.NextFollowUpDate.HasValue && t.NextFollowUpDate.Value.Date >= today && t.NextFollowUpDate.Value.Date <= next7Days)
                .Select(t => new
                {
                    t.TaskTitle,
                    Student = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : "",
                    Faculty = t.ProjectAllocation != null && t.ProjectAllocation.Faculty != null ? t.ProjectAllocation.Faculty.FullName : "",
                    FollowUpDate = t.NextFollowUpDate!.Value.ToString("dd-MMM-yyyy")
                })
                .ToListAsync();

            return Ok(result);
        }

        // 12) Show how many students have obtained each grade.
        [HttpGet("students-by-grade")]
        public async Task<IActionResult> GetStudentsByGrade()
        {
            var result = await _context.ProjectAllocations
                .Where(pa => !string.IsNullOrEmpty(pa.OverAllGrade))
                .GroupBy(pa => pa.OverAllGrade!)
                .Select(g => new { Grade = g.Key, Students = g.Select(pa => pa.StudentID).Distinct().Count() })
                .ToListAsync();

            return Ok(result);
        }

        // 13) Show month-wise completed task count.
        [HttpGet("month-wise-completed-tasks")]
        public async Task<IActionResult> GetMonthWiseCompletedTasks()
        {
            var tasks = await _context.Tasks
                .Include(t => t.TaskStatus)
                .Where(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") || t.TaskCompletedDate != null)
                .Where(t => t.TaskCompletedDate.HasValue)
                .ToListAsync();

            var result = tasks
                .GroupBy(t => new { t.TaskCompletedDate!.Value.Year, t.TaskCompletedDate.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    CompletedTasks = g.Count()
                });

            return Ok(result);
        }

        // 14) Display Role Wise Active User Count.
        [HttpGet("role-wise-active-users")]
        public async Task<IActionResult> GetRoleWiseActiveUsers()
        {
            var result = await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Where(ur => ur.User != null && ur.User.IsActive)
                .GroupBy(ur => ur.Role != null ? ur.Role.RoleName : "Unknown")
                .Select(g => new { Role = g.Key, ActiveUsers = g.Select(ur => ur.UserID).Distinct().Count() })
                .ToListAsync();

            return Ok(result);
        }

        // 15) Display each role with users assigned to it.
        [HttpGet("role-wise-users-list")]
        public async Task<IActionResult> GetRoleWiseUsersList()
        {
            var result = await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Select(ur => new
                {
                    Role = ur.Role != null ? ur.Role.RoleName : "Unknown",
                    UserName = ur.User != null ? ur.User.FullName : "Unknown"
                })
                .ToListAsync();

            return Ok(result);
        }

        // 16) List Roles Having More Than 10 Users.
        [HttpGet("roles-more-than-10-users")]
        public async Task<IActionResult> GetRolesMoreThan10Users()
        {
            var result = await _context.UserRoles
                .Include(ur => ur.Role)
                .GroupBy(ur => ur.Role != null ? ur.Role.RoleName : "Unknown")
                .Select(g => new { Role = g.Key, TotalUsers = g.Select(ur => ur.UserID).Distinct().Count() })
                .Where(x => x.TotalUsers > 10)
                .ToListAsync();

            return Ok(result);
        }

        // 17) Display role statistics.
        [HttpGet("role-statistics")]
        public async Task<IActionResult> GetRoleStatistics()
        {
            var result = await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .GroupBy(ur => ur.Role != null ? ur.Role.RoleName : "Unknown")
                .Select(g => new
                {
                    Role = g.Key,
                    TotalUsers = g.Select(ur => ur.UserID).Distinct().Count(),
                    ActiveUsers = g.Where(ur => ur.User != null && ur.User.IsActive).Select(ur => ur.UserID).Distinct().Count(),
                    InactiveUsers = g.Where(ur => ur.User != null && !ur.User.IsActive).Select(ur => ur.UserID).Distinct().Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        // 18) Show tasks due within next 7 days.
        [HttpGet("tasks-due-next-7-days")]
        public async Task<IActionResult> GetTasksDueNext7Days()
        {
            var today = DateTime.Today;
            var next7Days = today.AddDays(7);
            var result = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Project)
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Student)
                .Where(t => t.TaskDueDate.HasValue && t.TaskDueDate.Value.Date >= today && t.TaskDueDate.Value.Date <= next7Days)
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskTitle,
                    Project = t.ProjectAllocation != null && t.ProjectAllocation.Project != null ? t.ProjectAllocation.Project.ProjectTitle : "",
                    Student = t.ProjectAllocation != null && t.ProjectAllocation.Student != null ? t.ProjectAllocation.Student.FullName : "",
                    DueDate = t.TaskDueDate!.Value.ToString("dd-MMM-yyyy"),
                    DaysRemaining = (t.TaskDueDate.Value.Date - today).Days
                })
                .ToListAsync();

            return Ok(result);
        }

        // 19) Display each project with total tasks, completed tasks, pending tasks, and average task progress.
        [HttpGet("project-task-summary")]
        public async Task<IActionResult> GetProjectTaskSummary()
        {
            var projects = await _context.ProjectMasters.ToListAsync();
            var tasks = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                .Include(t => t.TaskStatus)
                .ToListAsync();

            var result = projects.Select(p =>
            {
                var pTasks = tasks.Where(t => t.ProjectAllocation != null && t.ProjectAllocation.ProjectID == p.ProjectID).ToList();
                var totalTasksCount = pTasks.Count;
                var completedCount = pTasks.Count(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") || t.ProgressPercentage == 100 || t.TaskCompletedDate != null);
                var pendingCount = totalTasksCount - completedCount;
                var avgProg = pTasks.Any() ? Math.Round(pTasks.Average(t => (double)t.ProgressPercentage), 2) : 0;

                return new
                {
                    Project = p.ProjectTitle,
                    Tasks = totalTasksCount,
                    Completed = completedCount,
                    Pending = pendingCount,
                    AvgProgress = avgProg + "%"
                };
            });

            return Ok(result);
        }

        // 20) Display project-wise total assigned score, earned score, and score percentage.
        [HttpGet("project-score-summary")]
        public async Task<IActionResult> GetProjectScoreSummary()
        {
            var projects = await _context.ProjectMasters.ToListAsync();
            var tasks = await _context.Tasks.Include(t => t.ProjectAllocation).ToListAsync();

            var result = projects.Select(p =>
            {
                var pTasks = tasks.Where(t => t.ProjectAllocation != null && t.ProjectAllocation.ProjectID == p.ProjectID).ToList();
                var assigned = pTasks.Sum(t => (double)t.AssignedScore);
                var earned = pTasks.Sum(t => (double)(t.EarnedScore ?? 0));
                var scorePct = assigned > 0 ? Math.Round((earned / assigned) * 100, 2) : 0;

                return new
                {
                    Project = p.ProjectTitle,
                    TotalAssignedScore = assigned,
                    TotalEarnedScore = earned,
                    ScorePercentage = scorePct.ToString("0.00") + "%"
                };
            });

            return Ok(result);
        }

        // 21) Display Top 10 projects based on average earned score.
        [HttpGet("top-10-projects-by-score")]
        public async Task<IActionResult> GetTop10ProjectsByScore()
        {
            var topProjects = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                    .ThenInclude(pa => pa!.Project)
                .Where(t => t.ProjectAllocation != null && t.ProjectAllocation.Project != null && t.EarnedScore.HasValue)
                .GroupBy(t => new { t.ProjectAllocation!.ProjectID, t.ProjectAllocation.Project!.ProjectTitle })
                .Select(g => new
                {
                    Project = g.Key.ProjectTitle,
                    AverageScore = Math.Round(g.Average(t => (double)t.EarnedScore!.Value), 2)
                })
                .OrderByDescending(x => x.AverageScore)
                .Take(10)
                .ToListAsync();

            var result = topProjects.Select((p, index) => new
            {
                Rank = index + 1,
                p.Project,
                p.AverageScore
            });

            return Ok(result);
        }

        // 22) Show project count, task count, and average progress for each faculty.
        [HttpGet("faculty-project-summary")]
        public async Task<IActionResult> GetFacultyProjectSummary()
        {
            var allocations = await _context.ProjectAllocations.Include(pa => pa.Faculty).ToListAsync();
            var tasks = await _context.Tasks.Include(t => t.ProjectAllocation).ToListAsync();

            var result = allocations
                .Where(pa => pa.Faculty != null)
                .GroupBy(pa => new { pa.FacultyID, pa.Faculty!.FullName })
                .Select(g =>
                {
                    var facTasks = tasks.Where(t => t.ProjectAllocation != null && t.ProjectAllocation.FacultyID == g.Key.FacultyID).ToList();
                    var avgProg = facTasks.Any() ? Math.Round(facTasks.Average(t => (double)t.ProgressPercentage), 2) : 0;

                    return new
                    {
                        Faculty = g.Key.FullName,
                        TotalProjects = g.Select(pa => pa.ProjectID).Distinct().Count(),
                        TotalTasks = facTasks.Count,
                        AvgProgress = avgProg
                    };
                });

            return Ok(result);
        }

        // 23) Display task completion statistics and average score for each student.
        [HttpGet("student-task-statistics")]
        public async Task<IActionResult> GetStudentTaskStatistics()
        {
            var allocations = await _context.ProjectAllocations.Include(pa => pa.Student).ToListAsync();
            var tasks = await _context.Tasks.Include(t => t.ProjectAllocation).Include(t => t.TaskStatus).ToListAsync();

            var result = allocations
                .Where(pa => pa.Student != null)
                .GroupBy(pa => new { pa.StudentID, pa.Student!.FullName })
                .Select(g =>
                {
                    var stuTasks = tasks.Where(t => t.ProjectAllocation != null && t.ProjectAllocation.StudentID == g.Key.StudentID).ToList();
                    var totalTasks = stuTasks.Count;
                    var completedTasks = stuTasks.Count(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") || t.ProgressPercentage == 100 || t.TaskCompletedDate != null);
                    var pendingTasks = totalTasks - completedTasks;
                    var scoredTasks = stuTasks.Where(t => t.EarnedScore.HasValue).ToList();
                    var avgScore = scoredTasks.Any() ? Math.Round(scoredTasks.Average(t => (double)t.EarnedScore!.Value), 2) : 0;

                    return new
                    {
                        Student = g.Key.FullName,
                        TotalTasks = totalTasks,
                        CompletedTasks = completedTasks,
                        PendingTasks = pendingTasks,
                        AvgScore = avgScore
                    };
                });

            return Ok(result);
        }

        // 24) Display projects whose expected completion date has passed but are still incomplete.
        [HttpGet("overdue-projects")]
        public async Task<IActionResult> GetOverdueProjects()
        {
            var today = DateTime.Today;
            var result = await _context.ProjectAllocations
                .Include(pa => pa.Project)
                .Include(pa => pa.Student)
                .Include(pa => pa.Faculty)
                .Where(pa => pa.ProjectEndDate < today && pa.ProgressPercentage < 100)
                .Select(pa => new
                {
                    Project = pa.Project != null ? pa.Project.ProjectTitle : "",
                    Student = pa.Student != null ? pa.Student.FullName : "",
                    Faculty = pa.Faculty != null ? pa.Faculty.FullName : "",
                    EndDate = pa.ProjectEndDate.ToString("dd-MMM-yyyy"),
                    Progress = pa.ProgressPercentage
                })
                .ToListAsync();

            return Ok(result);
        }

        // 25) Show month-wise completed task count (numeric month).
        [HttpGet("month-wise-completed-tasks-numeric")]
        public async Task<IActionResult> GetMonthWiseCompletedTasksNumeric()
        {
            var tasks = await _context.Tasks
                .Include(t => t.TaskStatus)
                .Where(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") || t.TaskCompletedDate != null)
                .Where(t => t.TaskCompletedDate.HasValue)
                .ToListAsync();

            var result = tasks
                .GroupBy(t => new { t.TaskCompletedDate!.Value.Year, t.TaskCompletedDate.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    CompletedTasks = g.Count()
                });

            return Ok(result);
        }

        // 26) Rank faculties based on average project progress.
        [HttpGet("rank-faculties-by-progress")]
        public async Task<IActionResult> GetRankFacultiesByProgress()
        {
            var allocations = await _context.ProjectAllocations
                .Include(pa => pa.Faculty)
                .Where(pa => pa.Faculty != null)
                .ToListAsync();

            var facultyProgress = allocations
                .GroupBy(pa => new { pa.FacultyID, pa.Faculty!.FullName })
                .Select(g => new
                {
                    Faculty = g.Key.FullName,
                    AvgProgress = Math.Round((double)g.Average(pa => pa.ProgressPercentage), 2)
                })
                .OrderByDescending(x => x.AvgProgress)
                .ToList();

            var result = facultyProgress.Select((f, index) => new
            {
                Rank = index + 1,
                f.Faculty,
                f.AvgProgress
            });

            return Ok(result);
        }

        // 27) Display task statistics for every project.
        [HttpGet("project-task-statistics")]
        public async Task<IActionResult> GetProjectTaskStatistics()
        {
            var today = DateTime.Today;
            var projects = await _context.ProjectMasters.ToListAsync();
            var tasks = await _context.Tasks
                .Include(t => t.ProjectAllocation)
                .Include(t => t.TaskStatus)
                .ToListAsync();

            var result = projects.Select(p =>
            {
                var pTasks = tasks.Where(t => t.ProjectAllocation != null && t.ProjectAllocation.ProjectID == p.ProjectID).ToList();
                var totalTasksCount = pTasks.Count;
                var completedCount = pTasks.Count(t => (t.TaskStatus != null && t.TaskStatus.TaskStatusName.ToLower() == "completed") || t.ProgressPercentage == 100 || t.TaskCompletedDate != null);
                var pendingCount = totalTasksCount - completedCount;
                var overdueCount = pTasks.Count(t => t.TaskDueDate.HasValue && t.TaskDueDate.Value.Date < today && ((t.TaskStatus == null || t.TaskStatus.TaskStatusName.ToLower() != "completed") && t.ProgressPercentage < 100));

                return new
                {
                    Project = p.ProjectTitle,
                    TotalTasks = totalTasksCount,
                    Completed = completedCount,
                    Pending = pendingCount,
                    Overdue = overdueCount
                };
            });

            return Ok(result);
        }

        #endregion
    }
}