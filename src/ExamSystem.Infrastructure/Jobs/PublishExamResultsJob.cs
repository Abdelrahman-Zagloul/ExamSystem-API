using ExamSystem.Application.Contracts.Jobs;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Jobs
{
    public class PublishExamResultsJob : IPublishExamResultsJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppEmailService _appEmailService;
        private record ExamResultDetails(string ExamTitle, string StudentName, string Email, double TotalMark, double Score);
        public PublishExamResultsJob(IUnitOfWork unitOfWork, IAppEmailService appEmailService)
        {
            _unitOfWork = unitOfWork;
            _appEmailService = appEmailService;
        }

        public async Task ExecuteAsync(int examId)
        {
            var exam = await _unitOfWork.Repository<Exam>().FindAsync(CancellationToken.None, examId);
            if (exam == null || exam.ResultsPublished)
                return;

            var examResultDetails = await _unitOfWork.Repository<ExamResult>()
                   .GetAsQuery(true).Where(x => x.ExamId == examId)
                   .Select(x => new ExamResultDetails(exam.Title, x.Student.FullName, x.Student.Email!, x.TotalMark, x.Score))
                   .ToListAsync();

            if (!examResultDetails.Any())
                return;

            foreach (var result in examResultDetails)
            {
                await _appEmailService.SendEmailForExamResultAsync
                    (result.ExamTitle, result.StudentName, result.TotalMark, result.Score, result.Email, examId);
            }

            exam.PublishExamResults();
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
