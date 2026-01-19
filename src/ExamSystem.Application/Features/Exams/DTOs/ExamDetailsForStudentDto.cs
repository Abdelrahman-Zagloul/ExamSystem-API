namespace ExamSystem.Application.Features.Exams.DTOs
{
    public class ExamDetailsForStudentDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime StartAt { get; init; }
        public DateTime EndAt { get; init; }
        public int DurationInMinutes { get; init; }
        public ExamStatus ExamStatus { get; init; }
        public int QuestionsCount { get; init; }
        public string DoctorName { get; init; } = null!;
    }
}
