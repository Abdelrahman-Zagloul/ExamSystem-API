namespace ExamSystem.Application.Features.Exams.DTOs
{
    public class StartExamResponseDto
    {
        public int ExamId { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime StartAt { get; init; }
        public DateTime EndAt { get; init; }
        public int DurationInMinutes { get; init; }
        public int QuestionsCount { get; init; }
        public List<ExamQuestionDto> Questions { get; init; } = [];
    }
}
