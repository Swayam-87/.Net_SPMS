namespace StudentProManagement.DTO_s
{
    public class UserRole_Admin_Response_DTO
    {
        public int RolePermissionID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

    public class UserRole_Create_DTO
    {
        public int RoleID { get; set; }
        public int UserID { get; set; }
    }

    public class UserRole_Update_DTO
    {
        public int RoleID { get; set; }
        public int UserID { get; set; }
    }
}
