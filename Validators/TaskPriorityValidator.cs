using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class TaskPriorityValidator : AbstractValidator<TaskPriority_Response_DTO>
    {
        public TaskPriorityValidator()
        {
            RuleFor(x => x.TaskPriorityName)
                .NotEmpty().WithMessage("Task Priority Name is required")
                .MaximumLength(50).WithMessage("Task Priority Name cannot exceed 50 characters");

            RuleFor(x => x.TaskPriorityCssClass)
                .NotEmpty().WithMessage("CSS Class is required")
                .MaximumLength(50).WithMessage("CSS Class cannot exceed 50 characters");
        }
    }
}
