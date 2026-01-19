using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Commands.StartExam
{
    public class StartExamCommandValidator : AbstractValidator<StartExamCommand>
    {
        public StartExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");
        }
    }
}
