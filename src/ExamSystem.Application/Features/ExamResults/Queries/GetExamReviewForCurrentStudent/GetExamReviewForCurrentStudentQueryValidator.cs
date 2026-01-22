using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent
{
    public class GetExamReviewForCurrentStudentQueryValidator : AbstractValidator<GetExamReviewForCurrentStudentQuery>
    {
        public GetExamReviewForCurrentStudentQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");
        }
    }
}
