using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Domain.Entities.Questions
{
    public class Question
    {
        public int Id { get; set; } // PK
        public string QuestionText { get; set; } = null!;
        public double QuestionMark { get; set; }
        public QuestionType QuestionType { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public int? CorrectOptionId { get; set; }
        public Option? CorrectOption { get; set; }
        public ICollection<Option> Options { get; set; } = [];
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
    }
}
