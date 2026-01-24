using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Domain.Entities.Exams
{
    public class StudentAnswer
    {
        public AnswerEvaluationStatus EvaluationStatus { get; private set; }

        public string StudentId { get; private set; } = null!;
        public Student Student { get; private set; } = null!;

        public int ExamId { get; private set; }
        public Exam Exam { get; private set; } = null!;

        public int QuestionId { get; private set; }
        public Question Question { get; private set; } = null!;

        public int SelectedOptionId { get; private set; }
        public Option Option { get; private set; } = null!;


        protected StudentAnswer() { }
        public StudentAnswer(int examId, string studentId, int questionId, int selectedOptionId)
        {
            ExamId = examId;
            StudentId = studentId;
            QuestionId = questionId;
            SelectedOptionId = selectedOptionId;
            EvaluationStatus = AnswerEvaluationStatus.Pending;
        }
        public void EvaluateAnswer(bool isCorrect)
        {
            EvaluationStatus = isCorrect
                ? AnswerEvaluationStatus.Correct
                : AnswerEvaluationStatus.Wrong;
        }
    }
}
