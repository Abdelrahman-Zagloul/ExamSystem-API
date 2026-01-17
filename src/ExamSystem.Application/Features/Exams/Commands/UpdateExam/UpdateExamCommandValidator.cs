using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Commands.UpdateExam
{
    public class UpdateExamCommandValidator : AbstractValidator<UpdateExamCommand>
    {
        public UpdateExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");

            RuleFor(x => x.Title)
                .NotEmpty().MaximumLength(200)
                .When(command => !string.IsNullOrEmpty(command.Title))
                .WithMessage("Title must not be empty and must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description))
                .WithMessage("Description must not exceed 1000 characters");

            RuleFor(x => x.StartAt)
                .GreaterThan(DateTime.UtcNow)
                .When(command => command.StartAt.HasValue)
                .WithMessage("Start date must be in the future");

            RuleFor(x => x.DurationInMinutes)
              .GreaterThan(0)
              .When(x => x.DurationInMinutes.HasValue)
              .WithMessage("Duration must be greater than 0.");

            RuleFor(x => x)
                  .Must(x =>
                  {
                      if (x.StartAt.HasValue && x.EndAt.HasValue)
                          return x.EndAt > x.StartAt;

                      return true;
                  })
                  .WithMessage("End date must be after start date.");

            RuleFor(x => x)
               .Must(x =>
               {
                   if (x.StartAt.HasValue && x.EndAt.HasValue && x.DurationInMinutes.HasValue)
                   {
                       var availableMinutes = (x.EndAt.Value - x.StartAt.Value).TotalMinutes;

                       return x.DurationInMinutes.Value <= availableMinutes;
                   }
                   return true;
               })
               .WithMessage("Duration must be within the exam time range.");

            RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrEmpty(x.Title) ||
                !string.IsNullOrEmpty(x.Description) ||
                x.StartAt.HasValue ||
                x.EndAt.HasValue ||
                x.DurationInMinutes.HasValue)
            .WithMessage("At least one field must be provided for update.");

        }
    }
}
