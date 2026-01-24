using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Domain.Entities.Exams
{
    public class ExamResult
    {
        public double TotalMark { get; private set; }
        public double Score { get; private set; }

        public string StudentId { get; private set; } = null!;
        public Student Student { get; private set; } = null!;
        public int ExamId { get; private set; }
        public Exam Exam { get; private set; } = null!;

        protected ExamResult() { }
        public ExamResult(string studentId, int examId, double totalMark, double score)
        {
            TotalMark = totalMark;
            Score = score;
            StudentId = studentId;
            ExamId = examId;
        }
    }
}
