using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPM.Models
{
    public class UserRole
    {
        [Key]
        public int RolePermissionID { get; set; }

        [ForeignKey("Role")]
        public int RoleID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

        public Role? Role { get; set; }

        public User? User { get; set; }
    }
}