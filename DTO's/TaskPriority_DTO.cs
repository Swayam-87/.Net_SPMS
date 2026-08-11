namespace StudentProManagement.DTO_s
{
    public class TaskPriority_Response_DTO
    {
        public int TaskPriorityID { get; set; }
        public string TaskPriorityName { get; set; }
        public string TaskPriorityCssClass { get; set; }
    }

    public class TaskPriority_Create_DTO
    {
        public string TaskPriorityName { get; set; }
        public string TaskPriorityCssClass { get; set; }
    }

    public class TaskPriority_Update_DTO
    {
        public string TaskPriorityName { get; set; }
        public string TaskPriorityCssClass { get; set; }
    }
}
