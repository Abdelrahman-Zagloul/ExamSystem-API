using ExamSystem.Application.Common.Results;

namespace ExamSystem.Application.Features.Questions.Commands.DeleteQuestion
{
    public record DeleteQuestionCommand(int ExamId, int QuestionId) : IResultRequest<string>;
}
