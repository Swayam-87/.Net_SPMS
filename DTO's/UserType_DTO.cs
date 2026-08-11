namespace StudentProManagement.DTO_s
{
    public class UserType_Admin_Response_DTO
    {
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public string? Description { get; set; }
    }

    public class UserType_Create_DTO
    {
        public string UserTypeName { get; set; }
        public string? Description { get; set; }
    }

    public class UserType_Update_DTO
    {
        public string UserTypeName { get; set; }
        public string? Description { get; set; }
    }
}
