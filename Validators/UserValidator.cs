using FluentValidation;
using StudentProManagement.DTO_s;

namespace StudentProManagement.Validators
{
    public class UserValidator : AbstractValidator<User_Admin_Response_DTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserTypeID).GreaterThan(0).WithMessage("User Type is required");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required").MaximumLength(150).WithMessage("Full Name cannot exceed 150 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters");
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage("Mobile Number is required").Matches(@"^[0-9]{10}$").WithMessage("Mobile Number must be 10 digits");
        }
    }

    public class UserCreateValidator : AbstractValidator<User_Create_DTO>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.UserTypeID).GreaterThan(0).WithMessage("User Type is required");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required").MaximumLength(150).WithMessage("Full Name cannot exceed 150 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters");
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage("Mobile Number is required").Matches(@"^[0-9]{10}$").WithMessage("Mobile Number must be 10 digits");
        }
    }

    public class UserUpdateValidator : AbstractValidator<User_Update_DTO>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.UserTypeID).GreaterThan(0).WithMessage("User Type is required");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required").MaximumLength(150).WithMessage("Full Name cannot exceed 150 characters");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid email format");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").MinimumLength(6).WithMessage("Password must be at least 6 characters");
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage("Mobile Number is required").Matches(@"^[0-9]{10}$").WithMessage("Mobile Number must be 10 digits");
        }
    }
}
