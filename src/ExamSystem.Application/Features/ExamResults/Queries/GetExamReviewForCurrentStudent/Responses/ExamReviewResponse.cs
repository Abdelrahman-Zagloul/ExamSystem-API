using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses
{
    public class ExamReviewResponse
    {
        public int ExamId { get; init; }
        public string ExamTitle { get; init; } = null!;
        public double ExamDegree { get; init; }
        public double StudentDegree { get; init; }
        public DateTime ExamDate { get; init; }
        public IReadOnlyList<ExamQuestionReviewResponse> Questions { get; init; } = [];
        public int TotalQuestions => Questions.Count;
        public int CorrectAnswers => Questions.Count(q => q.IsCorrect);
        public double Percentage => ExamDegree == 0 ? 0 : (StudentDegree / ExamDegree) * 100;
        public ExamResultStatus Status => Percentage >= 50 ? ExamResultStatus.Passed : ExamResultStatus.Failed;
    }
}
