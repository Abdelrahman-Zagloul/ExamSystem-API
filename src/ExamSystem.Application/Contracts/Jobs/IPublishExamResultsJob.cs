namespace ExamSystem.Application.Contracts.Jobs
{
    public interface IPublishExamResultsJob
    {
        Task ExecuteAsync(int examId);
    }
}
