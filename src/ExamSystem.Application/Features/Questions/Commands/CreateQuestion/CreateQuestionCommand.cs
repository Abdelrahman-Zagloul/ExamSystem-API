using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests;
using ExamSystem.Domain.Entities.Questions;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.CreateQuestion
{
    public record CreateQuestionCommand(
        int ExamId,
        string QuestionText,
        double QuestionMark,
        QuestionType QuestionType,
        List<CreateOptionRequest> Options,
        int CorrectOptionNumber) : IRequest<Result>;
}
