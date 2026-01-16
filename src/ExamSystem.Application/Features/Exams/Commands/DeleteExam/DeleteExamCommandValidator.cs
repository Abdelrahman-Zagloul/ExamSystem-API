using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Commands.DeleteExam
{
    public class DeleteExamCommandValidator : AbstractValidator<DeleteExamCommand>
    {
        public DeleteExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam ID is required.")
                .GreaterThan(0).WithMessage("Exam ID must be a positive integer.");
        }
    }
}
