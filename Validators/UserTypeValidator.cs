using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class UserTypeValidator : AbstractValidator<UserType_Admin_Response_DTO>
    {
        public UserTypeValidator()
        {
            RuleFor(x => x.UserTypeName).NotEmpty().WithMessage("User Type Name is required").Must(x => !x.Any(char.IsDigit)).WithMessage("User Type Name cannot contain digits").MaximumLength(50).WithMessage("User Type Name cannot exceed 50 characters");
            RuleFor(x => x.Description).MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
        }
    }

    public class UserTypeCreateValidator : AbstractValidator<UserType_Create_DTO>
    {
        public UserTypeCreateValidator()
        {
            RuleFor(x => x.UserTypeName).NotEmpty().WithMessage("User Type Name is required").Must(x => !x.Any(char.IsDigit)).WithMessage("User Type Name cannot contain digits").MaximumLength(50).WithMessage("User Type Name cannot exceed 50 characters");
            RuleFor(x => x.Description).MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
        }
    }

    public class UserTypeUpdateValidator : AbstractValidator<UserType_Update_DTO>
    {
        public UserTypeUpdateValidator()
        {
            RuleFor(x => x.UserTypeName).NotEmpty().WithMessage("User Type Name is required").Must(x => !x.Any(char.IsDigit)).WithMessage("User Type Name cannot contain digits").MaximumLength(50).WithMessage("User Type Name cannot exceed 50 characters");
            RuleFor(x => x.Description).MaximumLength(250).WithMessage("Description cannot exceed 250 characters");
        }
    }
}
