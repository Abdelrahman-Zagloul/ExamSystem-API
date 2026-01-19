using ExamSystem.Domain.Entities.Exams;

namespace ExamSystem.Application.Features.ExamResults.DTOs
{
    public class StudentExamResultForDoctorDto
    {
        public string ExamTitle { get; init; } = null!;
        public string StudentName { get; init; } = null!;
        public double ExamDegree { get; init; }
        public double StudentDegree { get; init; }
        public ExamResultStatus Status => (ExamDegree * 0.5) <= StudentDegree
                ? ExamResultStatus.Passed : ExamResultStatus.Failed;
    }
}
