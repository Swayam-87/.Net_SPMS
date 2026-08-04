using System.ComponentModel.DataAnnotations;

namespace SPM.Models
{
    public class UserType
    {
        [Key]
        public int UserTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserTypeName { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public ICollection<User>? Users { get; set; }
    }
}