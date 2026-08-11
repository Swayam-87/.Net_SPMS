namespace StudentProManagement.DTO_s
{
    public class Login_Request_DTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Login_Response_DTO
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserTypeName { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
