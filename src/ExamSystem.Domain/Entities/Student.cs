namespace ExamSystem.Domain.Entities
{
    public class Student : ApplicationUser
    {
        public ICollection<ExamResult> ExamResults { get; set; } = [];
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
        public ICollection<ExamSession> ExamSessions { get; set; } = [];
    }
}
