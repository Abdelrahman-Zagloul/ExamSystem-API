using FluentValidation;

namespace ExamSystem.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandValidation : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidation()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("FullName is required.")
                .MaximumLength(100).WithMessage("FullName must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match.");

            RuleFor(x => x.Role).IsInEnum().WithMessage("Invalid role specified.");
        }
    }
}
