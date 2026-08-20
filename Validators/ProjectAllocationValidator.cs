using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class ProjectAllocationValidator : AbstractValidator<ProjectAllocation_Admin_Response_DTO>
    {
        public ProjectAllocationValidator()
        {
            RuleFor(x => x.ProjectID).GreaterThan(0).WithMessage("Project is required");
            RuleFor(x => x.StudentID).GreaterThan(0).WithMessage("Student is required");
            RuleFor(x => x.FacultyID).GreaterThan(0).WithMessage("Faculty is required");
            RuleFor(x => x.ProjectStartDate).NotEmpty().WithMessage("Project Start Date is required");
            RuleFor(x => x.ProjectEndDate).NotEmpty().WithMessage("Project End Date is required").GreaterThanOrEqualTo(x => x.ProjectStartDate).WithMessage("End Date must be on or after Start Date");
            RuleFor(x => x.ProgressPercentage).InclusiveBetween(0, 100).WithMessage("Progress must be between 0 and 100");
        }
    }

    public class ProjectAllocationCreateValidator : AbstractValidator<ProjectAllocation_Create_DTO>
    {
        public ProjectAllocationCreateValidator()
        {
            RuleFor(x => x.ProjectID).GreaterThan(0).WithMessage("Project is required");
            RuleFor(x => x.StudentID).GreaterThan(0).WithMessage("Student is required");
            RuleFor(x => x.FacultyID).GreaterThan(0).WithMessage("Faculty is required");
            RuleFor(x => x.ProjectStartDate).NotEmpty().WithMessage("Project Start Date is required");
            RuleFor(x => x.ProjectEndDate).NotEmpty().WithMessage("Project End Date is required").GreaterThanOrEqualTo(x => x.ProjectStartDate).WithMessage("End Date must be on or after Start Date");
        }
    }

    public class ProjectAllocationUpdateValidator : AbstractValidator<ProjectAllocation_Update_DTO>
    {
        public ProjectAllocationUpdateValidator()
        {
            RuleFor(x => x.ProjectID).GreaterThan(0).WithMessage("Project is required");
            RuleFor(x => x.StudentID).GreaterThan(0).WithMessage("Student is required");
            RuleFor(x => x.FacultyID).GreaterThan(0).WithMessage("Faculty is required");
            RuleFor(x => x.ProjectStartDate).NotEmpty().WithMessage("Project Start Date is required");
            RuleFor(x => x.ProjectEndDate).NotEmpty().WithMessage("Project End Date is required").GreaterThanOrEqualTo(x => x.ProjectStartDate).WithMessage("End Date must be on or after Start Date");
            RuleFor(x => x.ProgressPercentage).InclusiveBetween(0, 100).WithMessage("Progress must be between 0 and 100");
        }
    }
}
