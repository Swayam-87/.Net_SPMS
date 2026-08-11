namespace StudentProManagement.DTO_s
{
    public class User_Admin_Response_DTO
    {
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public string FullName { get; set; }
        public string? UserCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string ProfilePicturePath { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class User_Faculty_Response_DTO
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string? UserCode { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string ProfilePicturePath { get; set; }
        public string UserTypeName { get; set; }
    }

    public class User_Student_Response_DTO
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfilePicturePath { get; set; }
        public string UserTypeName { get; set; }
    }

    public class User_Create_DTO
    {
        public int UserTypeID { get; set; }
        public string FullName { get; set; }
        public string? UserCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string ProfilePicturePath { get; set; }
    }

    public class User_Update_DTO
    {
        public int UserTypeID { get; set; }
        public string FullName { get; set; }
        public string? UserCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string ProfilePicturePath { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
