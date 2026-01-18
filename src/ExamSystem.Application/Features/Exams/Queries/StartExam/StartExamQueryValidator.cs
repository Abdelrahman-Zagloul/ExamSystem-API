using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Queries.StartExam
{
    public class StartExamQueryValidator : AbstractValidator<StartExamQuery>
    {
        public StartExamQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");
        }
    }
}
