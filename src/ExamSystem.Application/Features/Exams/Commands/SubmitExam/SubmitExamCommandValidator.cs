using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Commands.SubmitExam
{
    public class SubmitExamCommandValidator : AbstractValidator<SubmitExamCommand>
    {
        public SubmitExamCommandValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");


            RuleForEach(x => x.Answers)
            .NotNull().WithMessage("answer is required")
            .ChildRules(answer =>
            {
                answer.RuleFor(a => a.QuestionId)
                    .MustBePositiveNumber("Question ID");

                answer.RuleFor(a => a.SelectedOptionId)
                    .MustBePositiveNumber("Selected Option ID");
            });
        }
    }
}
