using ExamSystem.Domain.Enums;

namespace ExamSystem.Application.Features.Questions.DTOs
{
    public record CreateQuestionRequestDto
     (
        string QuestionText,
        double QuestionMark,
        QuestionType QuestionType,
        List<CreateOptionDto> Options,
        int CorrectOptionNumber
     );
}
