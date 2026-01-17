using ExamSystem.Application.Common.Validations;

namespace ExamSystem.Application.Features.Questions.Queries.GetAllQuestionsForDoctor
{
    public class GetExamQuestionsForDoctorQueryValidator : PaginationValidator<GetExamQuestionsForDoctorQuery>
    {
        public GetExamQuestionsForDoctorQueryValidator()
        {

            RuleFor(x => x.ExamId)
                    .MustBePositiveNumber("Exam ID");
        }
    }
}
