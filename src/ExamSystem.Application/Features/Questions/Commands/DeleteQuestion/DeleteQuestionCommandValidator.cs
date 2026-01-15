using FluentValidation;

namespace ExamSystem.Application.Features.Questions.Commands.DeleteQuestion
{
    public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
    {
        public DeleteQuestionCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam ID is required.")
                .GreaterThan(0).WithMessage("Exam ID must be a positive integer.");

            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage("Question ID is required.")
                .GreaterThan(0).WithMessage("Question ID must be a positive integer.");
        }
    }
}
