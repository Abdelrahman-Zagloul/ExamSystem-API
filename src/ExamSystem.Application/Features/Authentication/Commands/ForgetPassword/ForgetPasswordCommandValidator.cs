using FluentValidation;

namespace ExamSystem.Application.Features.Authentication.Commands.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("Email is required.")
               .EmailAddress().WithMessage("A valid email is required.");
        }
    }
}
