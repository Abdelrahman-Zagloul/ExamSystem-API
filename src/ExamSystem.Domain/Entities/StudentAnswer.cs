namespace ExamSystem.Domain.Entities
{
    public class StudentAnswer
    {
        public bool IsCorrect { get; set; }

        public string StudentId { get; set; } = null!;
        public Student Student { get; set; } = null!;

        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int SelectedOptionId { get; set; }
        public Option Option { get; set; } = null!;
    }
}
