using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SPM.Models
{
    public class ProjectMaster
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        public string ProjectTitle { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<ProjectAllocation>? ProjectAllocations { get; set; }
    }
}