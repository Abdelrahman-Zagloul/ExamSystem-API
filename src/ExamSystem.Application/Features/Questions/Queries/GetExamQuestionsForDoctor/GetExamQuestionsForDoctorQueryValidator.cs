using ExamSystem.Application.Common.PaginatedResult;
using FluentValidation;

namespace ExamSystem.Application.Features.Questions.Queries.GetAllQuestionsForDoctor
{
    public class GetExamQuestionsForDoctorQueryValidator : PaginationValidator<GetExamQuestionsForDoctorQuery>
    {
        public GetExamQuestionsForDoctorQueryValidator()
        {

            RuleFor(x => x.ExamId)
                .NotEmpty().WithMessage("Exam Id is required")
                .GreaterThan(0).WithMessage("Exam Id Must be Greater than 0");
        }
    }
}
