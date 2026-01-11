namespace ExamSystem.Domain.Entities
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
        public ICollection<Student> Students { get; set; } = [];
        public ICollection<Question> Questions { get; set; } = [];
        public ICollection<ExamResult> ExamResults { get; set; } = [];
    }
}
