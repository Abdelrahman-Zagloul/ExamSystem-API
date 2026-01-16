using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Commands.CreateExam
{
    public class CreateExamCommandValidator : AbstractValidator<CreateExamCommand>
    {
        public CreateExamCommandValidator()
        {
            RuleFor(x => x.Title)
             .NotEmpty().WithMessage("Title is required")
             .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.StartAt)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Start date must be in the future");

            RuleFor(x => x.EndAt)
                .GreaterThan(x => x.StartAt)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0")
                .Must((command, duration) =>
                {
                    var availableMinutes = (command.EndAt - command.StartAt).TotalMinutes;
                    return duration <= availableMinutes;
                }).WithMessage("Duration must be within the exam time range");
        }
    }
}
