using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPM.Models
{
    public class SPM_Task
    {
        [Key]
        public int TaskID { get; set; }

        [ForeignKey("ProjectAllocation")]
        public int ProjectAllocationID { get; set; }

        [Required]
        public string TaskTitle { get; set; }

        public string? TaskDescription { get; set; }

        [ForeignKey("TaskStatus")]
        public int TaskStatusID { get; set; }

        [ForeignKey("TaskPriority")]
        public int TaskPriorityID { get; set; }

        public decimal AssignedScore { get; set; }

        public decimal? EarnedScore { get; set; }

        public decimal ProgressPercentage { get; set; }

        public DateTime TaskAssignedDate { get; set; }

        public DateTime? TaskStartDate { get; set; }

        public DateTime? TaskDueDate { get; set; }

        public DateTime? TaskCompletedDate { get; set; }

        public DateTime? NextFollowUpDate { get; set; }

        public string? FacultyRemarks { get; set; }

        public string? StudentRemarks { get; set; }

        public ProjectAllocation? ProjectAllocation { get; set; }

        public TaskStatus_SPM? TaskStatus { get; set; }

        public TaskPriority? TaskPriority { get; set; }
    }
}