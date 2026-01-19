using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.ExamResults.DTOs
{
    public class StudentExamResultForStudentDto
    {
        public string ExamTitle { get; init; } = null!;
        public double ExamDegree { get; init; }
        public double StudentDegree { get; init; }
        public DateTime ExamDate { get; init; }
        public double Percentage => (StudentDegree / ExamDegree) * 100;
        public ExamResultStatus Status => (ExamDegree * 0.5) <= StudentDegree
                ? ExamResultStatus.Passed : ExamResultStatus.Failed;
    }
}
