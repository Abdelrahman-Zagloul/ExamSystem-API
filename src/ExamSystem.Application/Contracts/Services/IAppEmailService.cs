using ExamSystem.Domain.Entities;

namespace ExamSystem.Application.Contracts.Services
{
    public interface IAppEmailService
    {
        Task SendEmailForWelcomeMessageAsync(ApplicationUser user);
        Task SendEmailForConfirmEmailAsync(ApplicationUser user, string token);
        Task SendEmailForPasswordChangedAsync(ApplicationUser user);
        Task SendEmailForResetPasswordAsync(ApplicationUser user, string token);
    }
}
