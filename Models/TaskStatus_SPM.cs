using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SPM.Models
{
    public class TaskStatus_SPM
    {
        [Key]
        public int TaskStatusID { get; set; }

        [Required]
        public string TaskStatusName { get; set; }

        [Required]
        public string TaskStatusCssClass { get; set; }

        [JsonIgnore]
        public ICollection<SPM_Task>? Tasks { get; set; }
    }
}