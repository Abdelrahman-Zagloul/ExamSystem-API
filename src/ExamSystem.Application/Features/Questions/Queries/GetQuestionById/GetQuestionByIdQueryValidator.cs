using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Questions.Queries.GetQuestionById
{
    public class GetQuestionByIdQueryValidator : AbstractValidator<GetQuestionByIdQuery>
    {
        public GetQuestionByIdQueryValidator()
        {
            RuleFor(x => x.ExamId)
                .MustBePositiveNumber("Exam Id");

            RuleFor(x => x.QuestionId)
                .MustBePositiveNumber("Question Id");
        }
    }
}
