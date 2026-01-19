using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Domain.Entities.Questions;
using MediatR;

namespace ExamSystem.Application.Features.Questions.Commands.CreateQuestion
{
    public record CreateQuestionCommand(
        int ExamId,
        string QuestionText,
        double QuestionMark,
        QuestionType QuestionType,
        List<CreateOptionDto> Options,
        int CorrectOptionNumber) : IRequest<Result>;
}
