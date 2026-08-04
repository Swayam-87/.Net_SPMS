using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SPM.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [ForeignKey("UserType")]
        public int UserTypeID { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string? UserCode { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public string ProfilePicturePath { get; set; }

        public bool IsActive { get; set; }

        public bool? IsDeleted { get; set; }

        public UserType? UserType { get; set; }


        [JsonIgnore]
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}