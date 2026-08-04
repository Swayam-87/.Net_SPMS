using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SPM.Models
{
    public class TaskPriority
    {
        [Key]
        public int TaskPriorityID { get; set; }

        [Required]
        public string TaskPriorityName { get; set; }

        [Required]
        public string TaskPriorityCssClass { get; set; }

        [JsonIgnore]
        public ICollection<SPM_Task>? Tasks { get; set; }
    }
}