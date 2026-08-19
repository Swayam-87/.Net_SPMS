using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class SPM_TaskValidator : AbstractValidator<SPM_Task_Admin_Response_DTO>
    {
        public SPM_TaskValidator()
        {
            RuleFor(x => x.ProjectAllocationID)
                .GreaterThan(0).WithMessage("Project Allocation is required");

            RuleFor(x => x.TaskTitle)
                .NotEmpty().WithMessage("Task Title is required")
                .MaximumLength(200).WithMessage("Task Title cannot exceed 200 characters");

            RuleFor(x => x.TaskStatusID)
                .GreaterThan(0).WithMessage("Task Status is required");

            RuleFor(x => x.TaskPriorityID)
                .GreaterThan(0).WithMessage("Task Priority is required");

            RuleFor(x => x.AssignedScore)
                .GreaterThanOrEqualTo(0).WithMessage("Assigned Score cannot be negative");

            RuleFor(x => x.ProgressPercentage)
                .InclusiveBetween(0, 100).WithMessage("Progress must be between 0 and 100");
        }
    }
}
