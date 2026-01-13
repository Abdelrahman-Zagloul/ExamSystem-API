using FluentValidation;

namespace ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailValidator : AbstractValidator<ResendConfirmEmailCommand>
    {
        public ResendConfirmEmailValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        }
    }
}
