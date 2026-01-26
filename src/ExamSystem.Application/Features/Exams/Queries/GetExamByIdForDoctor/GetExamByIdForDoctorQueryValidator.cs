using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor
{
    public class GetExamByIdForDoctorQueryValidator : AbstractValidator<GetExamByIdForDoctorQuery>
    {
        public GetExamByIdForDoctorQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");
        }
    }
}
