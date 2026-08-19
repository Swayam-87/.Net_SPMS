using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class TaskStatusValidator : AbstractValidator<TaskStatus_SPM_Response_DTO>
    {
        public TaskStatusValidator()
        {
            RuleFor(x => x.TaskStatusName)
                .NotEmpty().WithMessage("Task Status Name is required")
                .MaximumLength(50).WithMessage("Task Status Name cannot exceed 50 characters");

            RuleFor(x => x.TaskStatusCssClass)
                .NotEmpty().WithMessage("CSS Class is required")
                .MaximumLength(50).WithMessage("CSS Class cannot exceed 50 characters");
        }
    }
}
