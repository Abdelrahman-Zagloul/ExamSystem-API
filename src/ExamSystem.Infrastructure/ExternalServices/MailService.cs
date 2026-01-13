using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ExamSystem.Infrastructure.ExternalServices
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;
        public MailService(IOptions<MailSettings> emailSettings, ILogger<MailService> logger)
        {
            _mailSettings = emailSettings.Value;
            _logger = logger;
        }
        public async Task SendEmailAsync(string to, string subject, string body, List<IFormFile>? attachments = null)
        {
            using var message = await CreateMessage(to, subject, body, attachments);

            using var smtpClient = new SmtpClient(_mailSettings.Host, _mailSettings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_mailSettings.Email, _mailSettings.Password)
            };

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed to send email to {To}", to);
            }


        }
        private async Task<MailMessage> CreateMessage(string to, string subject, string body, List<IFormFile>? attachments)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_mailSettings.Email, _mailSettings.DisplayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            message.To.Add(new MailAddress(to));

            // Add attachments
            if (attachments != null && attachments.Any())
            {
                foreach (var file in attachments.Where(x => x.Length > 0))
                {
                    var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    message.Attachments.Add(new Attachment(stream, file.FileName, file.ContentType));
                }
            }
            return message;
        }
    }
}
