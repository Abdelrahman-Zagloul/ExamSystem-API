using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor
{
    public class GetExamResultsForDoctorQueryValidator : AbstractValidator<GetExamResultsForDoctorQuery>
    {
        public GetExamResultsForDoctorQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");

            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("invalid exam result status");

            Include(new PaginationValidator<GetExamResultsForDoctorQuery>());
        }
    }
}
