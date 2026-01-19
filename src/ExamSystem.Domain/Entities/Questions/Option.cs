using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Domain.Entities.Questions
{
    public class Option
    {
        public int Id { get; set; }
        public string OptionText { get; set; } = null!;

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;
        public Question? QuestionAsCorrectOption { get; set; }
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
    }
}
