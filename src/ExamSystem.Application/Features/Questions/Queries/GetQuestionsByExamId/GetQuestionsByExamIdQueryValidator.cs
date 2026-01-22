using ExamSystem.Application.Common.Validations;

namespace ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId
{
    public class GetQuestionsByExamIdQueryValidator : PaginationValidator<GetQuestionsByExamIdQuery>
    {
        public GetQuestionsByExamIdQueryValidator()
        {

            RuleFor(x => x.ExamId)
                    .MustBePositiveNumber("Exam ID");
        }
    }
}
