using FluentValidation;

namespace ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailCommandValidator : AbstractValidator<ResendConfirmEmailCommand>
    {
        public ResendConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        }
    }
}
