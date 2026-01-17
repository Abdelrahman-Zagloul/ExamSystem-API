using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor
{
    internal class GetExamByIdForDoctorQueryValidator : AbstractValidator<GetExamByIdForDoctorQuery>
    {
        public GetExamByIdForDoctorQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam ID");
        }
    }
}
