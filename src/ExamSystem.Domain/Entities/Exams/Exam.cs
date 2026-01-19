using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Domain.Entities.Exams
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int DurationInMinutes { get; set; }

        public string DoctorId { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public ICollection<Question> Questions { get; set; } = [];
        public ICollection<ExamResult> ExamResults { get; set; } = [];
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
        public ICollection<ExamSession> ExamSessions { get; set; } = [];
    }
}
