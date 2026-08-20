using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class UserRoleValidator : AbstractValidator<UserRole_Admin_Response_DTO>
    {
        public UserRoleValidator()
        {
            RuleFor(x => x.RoleID).GreaterThan(0).WithMessage("Role is required");
            RuleFor(x => x.UserID).GreaterThan(0).WithMessage("User is required");
        }
    }

    public class UserRoleCreateValidator : AbstractValidator<UserRole_Create_DTO>
    {
        public UserRoleCreateValidator()
        {
            RuleFor(x => x.RoleID).GreaterThan(0).WithMessage("Role is required");
            RuleFor(x => x.UserID).GreaterThan(0).WithMessage("User is required");
        }
    }

    public class UserRoleUpdateValidator : AbstractValidator<UserRole_Update_DTO>
    {
        public UserRoleUpdateValidator()
        {
            RuleFor(x => x.RoleID).GreaterThan(0).WithMessage("Role is required");
            RuleFor(x => x.UserID).GreaterThan(0).WithMessage("User is required");
        }
    }
}
