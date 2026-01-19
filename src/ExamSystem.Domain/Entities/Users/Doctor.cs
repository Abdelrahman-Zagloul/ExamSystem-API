using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Domain.Entities.Users
{
    public class Doctor : ApplicationUser
    {
        public ICollection<Exam> CreatedExams { get; set; } = [];
    }
}
