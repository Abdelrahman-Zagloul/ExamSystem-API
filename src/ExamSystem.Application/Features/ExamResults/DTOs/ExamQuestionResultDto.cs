using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.ExamResults.DTOs
{
    public class ExamQuestionResultDto
    {
        public int QuestionId { get; init; }
        public string QuestionText { get; init; } = null!;
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionDto> Options { get; init; } = [];
        public OptionDto CorrectOption { get; init; } = null!;
        public OptionDto? StudentOption { get; set; }
        public bool IsCorrect => CorrectOption.OptionId == StudentOption?.OptionId;
    }
}
