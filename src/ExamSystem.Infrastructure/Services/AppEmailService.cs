using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Settings;
using ExamSystem.Domain.Entities.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ExamSystem.Infrastructure.Services
{
    public class AppEmailService : IAppEmailService
    {
        private readonly IMailService _mailService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly FrontendURLsSettings _uRLsSettings;

        public AppEmailService(IMailService mailService,
                               IWebHostEnvironment webHostEnvironment,
                               IOptions<FrontendURLsSettings> uRLsSettings)
        {
            _mailService = mailService;
            _webHostEnvironment = webHostEnvironment;
            _uRLsSettings = uRLsSettings.Value;
        }

        public async Task SendEmailForWelcomeMessageAsync(ApplicationUser user)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "WelcomeEmail.html");
            var templateContent = await File.ReadAllTextAsync(path);
            var emailBody = templateContent
                .Replace("{FullName}", user.FullName);

            await _mailService.SendEmailAsync(user.Email!, "Welcome to Exam System", emailBody, null);
        }
        public async Task SendEmailForConfirmEmailAsync(ApplicationUser user, string encodedToken)
        {
            var confirmationLink = $"{_uRLsSettings.ConfirmEmailPath}?email={user.Email}&token={encodedToken}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "ConfirmEmail.html");
            var templateContent = await File.ReadAllTextAsync(path);

            var emailBody = templateContent
                .Replace("{FullName}", user.FullName)
                .Replace("{confirmationLink}", confirmationLink)
                .Replace("{token}", encodedToken);

            await _mailService.SendEmailAsync(user.Email!, "Confirm Email", emailBody, null);
        }

        public async Task SendEmailForPasswordChangedAsync(ApplicationUser user)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "PasswordChanged.html");
            var templateContent = await File.ReadAllTextAsync(path);
            var emailBody = templateContent.Replace("{FullName}", user.FullName);

            await _mailService.SendEmailAsync(user.Email!, "Password Changed Successfully", emailBody, null);
        }

        public async Task SendEmailForResetPasswordAsync(ApplicationUser user, string EncodedToken)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "ResetPassword.html");

            var resetLink = $"{_uRLsSettings.ResetPasswordPath}?email={user.Email}&token={EncodedToken}";

            var templateContent = await File.ReadAllTextAsync(path);
            var emailBody = templateContent
                .Replace("{FullName}", user.FullName)
                .Replace("{ResetLink}", resetLink)
                .Replace("{Token}", EncodedToken);

            await _mailService.SendEmailAsync(user.Email!, "Reset Password", emailBody, null);
        }

        public async Task SendEmailForExamResultAsync(string examTitle, string fullName, double totalMark, double score, string email, int examId)
        {
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "EmailTemplates", "ExamResultEmail.html");

            var reviewUrl = $"{_uRLsSettings.ReviewExamResultPath.Replace("id", examId.ToString())}";

            var templateContent = await File.ReadAllTextAsync(path);
            var emailBody = templateContent
                .Replace("{FullName}", fullName)
                .Replace("{ExamTitle}", examTitle)
                .Replace("{Score}", score.ToString("0.##", CultureInfo.InvariantCulture))
                .Replace("{TotalMark}", totalMark.ToString("0.##", CultureInfo.InvariantCulture))
                .Replace("{ReviewUrl}", reviewUrl);
            await _mailService.SendEmailAsync(email, $"Exam Result - {examTitle}", emailBody, null);
        }
    }
}
