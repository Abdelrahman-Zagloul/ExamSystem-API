using ExamSystem.Application.Common.Validations;
using FluentValidation;

namespace ExamSystem.Application.Features.Questions.Commands.DeleteQuestion
{
    public class DeleteQuestionCommandValidator : AbstractValidator<DeleteQuestionCommand>
    {
        public DeleteQuestionCommandValidator()
        {
            RuleFor(x => x.ExamId)
              .MustBePositiveNumber("Exam ID ");

            RuleFor(x => x.QuestionId)
              .MustBePositiveNumber("Question ID ");
        }
    }
}
