using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Domain.Entities.Exams
{
    public class ExamResult
    {
        public double TotalMark { get; set; }
        public double Score { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;
    }
}
