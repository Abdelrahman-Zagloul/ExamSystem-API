using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Domain.Entities.Exams
{
    public class ExamSession
    {
        public DateTime StartedAt { get; private set; }
        public DateTime? SubmittedAt { get; private set; }

        public int ExamId { get; private set; }
        public Exam Exam { get; private set; } = null!;

        public string StudentId { get; private set; } = null!;
        public Student Student { get; private set; } = null!;

        protected ExamSession() { }

        public ExamSession(int examId, string studentId)
        {
            ExamId = examId;
            StudentId = studentId;
            StartedAt = DateTime.UtcNow;
        }

        public void SubmitSession()
        {
            if (SubmittedAt != null)
                throw new InvalidOperationException("Exam already submitted.");

            SubmittedAt = DateTime.UtcNow;
        }
    }
}
