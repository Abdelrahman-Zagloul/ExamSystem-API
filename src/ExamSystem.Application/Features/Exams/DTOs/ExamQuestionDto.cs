using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Domain.Enums;

namespace ExamSystem.Application.Features.Exams.DTOs
{
    public class ExamQuestionDto
    {
        public int QuestionId { get; init; }
        public string QuestionText { get; init; } = null!;
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionDto> Options { get; init; } = [];
    }
}
