using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent
{
    public class GetExamResultsForCurrentStudentQueryValidator : AbstractValidator<GetExamResultsForCurrentStudentQuery>
    {
        public GetExamResultsForCurrentStudentQueryValidator()
        {
            Include(new PaginationValidator<GetExamResultsForCurrentStudentQuery>());
        }
    }
}
