using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.Questions.Shared
{
    public class QuestionResponse
    {
        public int QuestionId { get; init; }
        public string QuestionText { get; init; } = null!;
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionResponse>? Options { get; init; }
        public OptionResponse? CorrectOption { get; init; }
        public string? ExamTitle { get; init; }
    }
}
