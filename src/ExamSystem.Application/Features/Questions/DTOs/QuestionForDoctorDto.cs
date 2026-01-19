using ExamSystem.Domain.Entities.Questions;

namespace ExamSystem.Application.Features.Questions.DTOs
{
    public class QuestionForDoctorDto
    {
        public int QuestionId { get; init; }
        public string? QuestionText { get; init; }
        public double QuestionMark { get; init; }
        public QuestionType QuestionType { get; init; }
        public List<OptionDto>? Options { get; init; }
        public OptionDto? CorrectOption { get; init; }
        public string? ExamTitle { get; init; }
    }
}
