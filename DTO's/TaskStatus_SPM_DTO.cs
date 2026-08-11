namespace StudentProManagement.DTO_s
{
    public class TaskStatus_SPM_Response_DTO
    {
        public int TaskStatusID { get; set; }
        public string TaskStatusName { get; set; }
        public string TaskStatusCssClass { get; set; }
    }

    public class TaskStatus_SPM_Create_DTO
    {
        public string TaskStatusName { get; set; }
        public string TaskStatusCssClass { get; set; }
    }

    public class TaskStatus_SPM_Update_DTO
    {
        public string TaskStatusName { get; set; }
        public string TaskStatusCssClass { get; set; }
    }
}
