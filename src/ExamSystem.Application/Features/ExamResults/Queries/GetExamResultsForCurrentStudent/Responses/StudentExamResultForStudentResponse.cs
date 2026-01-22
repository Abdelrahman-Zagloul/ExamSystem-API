using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent.Responses
{
    public class StudentExamResultForStudentResponse
    {
        public string ExamTitle { get; init; } = null!;
        public double ExamDegree { get; init; }
        public double StudentDegree { get; init; }
        public DateTime ExamDate { get; init; }
        public double Percentage => ExamDegree == 0 ? 0 : (StudentDegree / ExamDegree) * 100;
        public ExamResultStatus Status => (ExamDegree * 0.5) <= StudentDegree
                ? ExamResultStatus.Passed : ExamResultStatus.Failed;
    }
}
