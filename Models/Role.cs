using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SPM.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}