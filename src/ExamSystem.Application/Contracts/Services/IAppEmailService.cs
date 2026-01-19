using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Application.Contracts.Services
{
    public interface IAppEmailService
    {
        Task SendEmailForWelcomeMessageAsync(ApplicationUser user);
        Task SendEmailForConfirmEmailAsync(ApplicationUser user, string EncodedToken);
        Task SendEmailForPasswordChangedAsync(ApplicationUser user);
        Task SendEmailForResetPasswordAsync(ApplicationUser user, string EncodedToken);
    }
}
