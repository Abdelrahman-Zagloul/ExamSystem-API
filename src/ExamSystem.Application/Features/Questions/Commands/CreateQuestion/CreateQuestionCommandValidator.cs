using ExamSystem.Application.Common.Validations;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using FluentValidation;

namespace ExamSystem.Application.Features.Questions.Command.CreateQuestion
{
    public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
    {
        public CreateQuestionCommandValidator()
        {
            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(1000).WithMessage("Question text cannot exceed 1000 characters.");

            RuleFor(x => x.QuestionMark)
                .GreaterThan(0).WithMessage("Question mark must be greater than zero.");

            RuleFor(x => x.QuestionType)
                .IsInEnum().WithMessage("Invalid question type.");

            RuleFor(x => x.ExamId)
                    .MustBePositiveNumber("Exam ID");

            RuleFor(x => x.Options)
                .NotEmpty().WithMessage("Options are required.")
                .Must(options => options != null && options.Count >= 2).WithMessage("At least two options are required.");

            RuleFor(x => x.CorrectOptionNumber)
                .NotEmpty().WithMessage("Correct option number is required.")
                .GreaterThan(0).WithMessage("Correct option number must be greater than zero.")
                .Must((command, correctOptionNumber) =>
                {
                    return correctOptionNumber <= command.Options.Count;
                }).WithMessage("Correct option number must on of options.");
        }
    }
}
