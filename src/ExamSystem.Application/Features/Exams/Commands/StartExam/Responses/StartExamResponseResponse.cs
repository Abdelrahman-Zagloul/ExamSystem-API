namespace ExamSystem.Application.Features.Exams.Commands.StartExam.Responses
{
    public record StartExamResponse
    {
        public int ExamId { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime StartAt { get; init; }
        public DateTime EndAt { get; init; }
        public int DurationInMinutes { get; init; }
        public int QuestionsCount { get; init; }
        public IReadOnlyList<ExamQuestionResponse> Questions { get; init; } = [];
    }
}
