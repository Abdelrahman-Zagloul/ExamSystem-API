namespace ExamSystem.Domain.Entities
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
