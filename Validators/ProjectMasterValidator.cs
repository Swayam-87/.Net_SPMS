using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class ProjectMasterValidator : AbstractValidator<ProjectMaster_Admin_Response_DTO>
    {
        public ProjectMasterValidator()
        {
            RuleFor(x => x.ProjectTitle)
                .NotEmpty().WithMessage("Project Title is required")
                .MaximumLength(200).WithMessage("Project Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
        }
    }
}
