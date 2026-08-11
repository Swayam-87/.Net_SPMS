namespace StudentProManagement.DTO_s
{
    public class Role_Admin_Response_DTO
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }

    public class Role_Create_DTO
    {
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }

    public class Role_Update_DTO
    {
        public string RoleName { get; set; }
        public string? Description { get; set; }
    }
}
