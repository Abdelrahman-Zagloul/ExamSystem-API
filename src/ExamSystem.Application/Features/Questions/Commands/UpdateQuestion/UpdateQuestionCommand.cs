using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.UpdateQuestion
{
    public record UpdateQuestionCommand(
        int ExamId,
        int QuestionId,
        string? QuestionText,
        double? NewQuestionMark,
        List<UpdateOptionRequest>? Options,
        int? NewCorrectOptionId) : IRequest<Result>;
}
