using Microsoft.AspNetCore.Http;

namespace ExamSystem.Application.Contracts.ExternalServices
{
    public interface IMailService
    {
        Task SendEmailAsync(string to, string subject, string body, List<IFormFile>? attachments = null);
    }
}
