namespace ExamSystem.Domain.Entities
{
    public class Student : ApplicationUser
    {
        public ICollection<ExamResult> ExamResults { get; set; } = [];
        public ICollection<StudentAnswer> Answers { get; set; } = [];
    }
}
