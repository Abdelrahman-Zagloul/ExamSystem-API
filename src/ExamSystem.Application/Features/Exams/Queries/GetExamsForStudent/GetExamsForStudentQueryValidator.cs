using FluentValidation;

namespace ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent
{
    public class GetExamsForStudentQueryValidator : AbstractValidator<GetExamsForStudentQuery>
    {
        public GetExamsForStudentQueryValidator()
        {
            RuleFor(x => x.ExamStatus)
                .IsInEnum()
                .When(x => x.ExamStatus.HasValue)
                .WithMessage("invalid exam status");
        }
    }
}
