using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SPM.Models
{
    public class ProjectAllocation
    {
        [Key]
        public int ProjectAllocationID { get; set; }

        [ForeignKey("Project")]
        public int ProjectID { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }

        [ForeignKey("Faculty")]
        public int FacultyID { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime ProjectStartDate { get; set; }

        public DateTime ProjectEndDate { get; set; }

        public int TotalTasksGiven { get; set; }

        public int TotalCompletedTasks { get; set; }

        public decimal ProgressPercentage { get; set; }

        public string? OverAllGrade { get; set; }

        public ProjectMaster? Project { get; set; }

        public User? Student { get; set; }

        public User? Faculty { get; set; }

        [JsonIgnore]
        public ICollection<SPM_Task>? Tasks { get; set; }
    }
}