namespace ExamSystem.Domain.Entities
{
    public class ExamSession
    {
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;
    }
}
