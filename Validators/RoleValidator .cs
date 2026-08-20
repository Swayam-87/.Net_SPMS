using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class RoleValidator : AbstractValidator<Role_Admin_Response_DTO>
    {
        public RoleValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role Name is required")
                .Must(x => !x.Any(char.IsDigit)).WithMessage("Role Name cannot contain digits")
                .MaximumLength(50).WithMessage("Role Name cannot exceed 50 characters");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
        }
    }

    public class RoleCreateValidator : AbstractValidator<Role_Create_DTO>
    {
        public RoleCreateValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role Name is required")
                .Must(x => !x.Any(char.IsDigit)).WithMessage("Role Name cannot contain digits")
                .MaximumLength(50).WithMessage("Role Name cannot exceed 50 characters");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
        }
    }

    public class RoleUpdateValidator : AbstractValidator<Role_Update_DTO>
    {
        public RoleUpdateValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role Name is required")
                .Must(x => !x.Any(char.IsDigit)).WithMessage("Role Name cannot contain digits")
                .MaximumLength(50).WithMessage("Role Name cannot exceed 50 characters");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
        }
    }
}
