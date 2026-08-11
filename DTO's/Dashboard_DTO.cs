namespace StudentProManagement.DTO_s
{
    public class Dashboard_Admin_DTO
    {
        public int TotalStudents { get; set; }
        public int TotalFaculty { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int TotalCompletedTasks { get; set; }
        public int TotalPendingTasks { get; set; }
    }

    public class Dashboard_Faculty_DTO
    {
        public int TotalAssignedStudents { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public int TotalPendingTasks { get; set; }
    }

    public class Dashboard_Student_DTO
    {
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }
        public int TotalCompletedTasks { get; set; }
        public int TotalPendingTasks { get; set; }
        public decimal OverallProgress { get; set; }
    }

  
}
