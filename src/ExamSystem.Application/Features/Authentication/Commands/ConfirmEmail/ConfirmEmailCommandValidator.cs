using FluentValidation;

namespace ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("invalid Email format")
                .MaximumLength(200).WithMessage("email must be less than 200 latter");

            RuleFor(x => x.Token)
                   .NotEmpty().WithMessage("Token is required");
        }
    }
}
