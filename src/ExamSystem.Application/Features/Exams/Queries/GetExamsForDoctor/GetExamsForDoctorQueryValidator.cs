using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor
{
    public class GetExamsForDoctorQueryValidator : AbstractValidator<GetExamsForDoctorQuery>
    {
        public GetExamsForDoctorQueryValidator()
        {
            RuleFor(x => x.ExamStatus).IsInEnum()
                .When(x => x.ExamStatus.HasValue)
                .WithMessage("Invalid exam status value.");

            Include(new PaginationValidator<GetExamsForDoctorQuery>());
        }
    }
}
