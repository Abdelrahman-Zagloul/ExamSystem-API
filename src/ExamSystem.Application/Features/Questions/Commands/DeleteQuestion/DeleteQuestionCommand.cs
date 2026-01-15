using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.DeleteQuestion
{
    public record DeleteQuestionCommand(int ExamId, int QuestionId) : IRequest<Result>;
}
