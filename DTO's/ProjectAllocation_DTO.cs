namespace StudentProManagement.DTO_s
{
    public class ProjectAllocation_Admin_Response_DTO
    {
        public int ProjectAllocationID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public int FacultyID { get; set; }
        public string FacultyName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? OverAllGrade { get; set; }
    }

    public class ProjectAllocation_Faculty_Response_DTO
    {
        public int ProjectAllocationID { get; set; }
        public int ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? OverAllGrade { get; set; }
    }

    public class ProjectAllocation_Student_Response_DTO
    {
        public int ProjectAllocationID { get; set; }
        public string ProjectTitle { get; set; }
        public string FacultyName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? OverAllGrade { get; set; }
    }

    public class ProjectAllocation_Create_DTO
    {
        public int ProjectID { get; set; }
        public int StudentID { get; set; }
        public int FacultyID { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
    }

    public class ProjectAllocation_Update_DTO
    {
        public int ProjectID { get; set; }
        public int StudentID { get; set; }
        public int FacultyID { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public int TotalTasksGiven { get; set; }
        public int TotalCompletedTasks { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string? OverAllGrade { get; set; }
    }
}
