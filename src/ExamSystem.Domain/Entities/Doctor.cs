namespace ExamSystem.Domain.Entities
{
    public class Doctor : ApplicationUser
    {
        public ICollection<Exam> CreatedExams { get; set; } = [];
    }
}
