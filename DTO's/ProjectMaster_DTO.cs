namespace StudentProManagement.DTO_s
{
    public class ProjectMaster_Admin_Response_DTO
    {
        public int ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public string? Description { get; set; }
    }

    public class ProjectMaster_Faculty_Response_DTO
    {
        public int ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public string? Description { get; set; }
    }

    public class ProjectMaster_Student_Response_DTO
    {
        public int ProjectID { get; set; }
        public string ProjectTitle { get; set; }
        public string? Description { get; set; }
    }

    public class ProjectMaster_Create_DTO
    {
        public string ProjectTitle { get; set; }
        public string? Description { get; set; }
    }

    public class ProjectMaster_Update_DTO
    {
        public string ProjectTitle { get; set; }
        public string? Description { get; set; }
    }
}
