namespace ExamSystem.Application.Contracts.Jobs
{
    public interface ICalculateExamResultJob
    {
        Task ExecuteAsync(int examId, string studentId);
    }
}
