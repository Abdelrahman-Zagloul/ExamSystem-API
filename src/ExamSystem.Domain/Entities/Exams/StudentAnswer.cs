using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Domain.Entities.Exams
{
    public class StudentAnswer
    {
        public AnswerEvaluationStatus EvaluationStatus { get; set; } = AnswerEvaluationStatus.Pending;

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
