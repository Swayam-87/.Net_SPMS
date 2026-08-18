using FluentValidation;
using StudentProManagement.DTO_s;
namespace StudentProManagement.Validators;
    public class RoleValidator : AbstractValidator<Role_Admin_Response_DTO>
    {
        public RoleValidator() 
        {
            RuleFor(x => x.RoleName)

                // Name is mandatory
                .NotEmpty()
                .WithMessage("Student Name is required")

                // Name should not contain only whitespace characters
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Student Name cannot be empty or whitespace")

                //Name cannot contain numbers
                .Must(name => !name.Any(char.IsDigit))
                .WithMessage("Student Name cannot contain Digits")

                // Name length must not exceed 100 characters
                .MaximumLength(100)
                .WithMessage("Student Name cannot exceed 100 characters");
        }
    }

