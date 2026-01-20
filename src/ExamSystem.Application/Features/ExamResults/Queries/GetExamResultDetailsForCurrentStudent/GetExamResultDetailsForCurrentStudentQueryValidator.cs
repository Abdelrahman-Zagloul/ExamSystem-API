using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultDetailsForCurrentStudent
{
    public class GetExamResultDetailsForCurrentStudentQueryValidator : AbstractValidator<GetExamResultDetailsForCurrentStudentQuery>
    {
        public GetExamResultDetailsForCurrentStudentQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");
        }
    }
}
