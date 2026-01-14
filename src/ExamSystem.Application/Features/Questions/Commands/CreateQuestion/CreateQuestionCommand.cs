using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Application.Features.Questions.Commands.CreateQuestion
{
    public record CreateQuestionCommand(
        string QuestionText,
        double QuestionMark,
        QuestionType QuestionType,
        int ExamId,
        List<CreateOptionDto> Options,
        int CorrectOptionNumber) : IResultRequest<string>;
}
