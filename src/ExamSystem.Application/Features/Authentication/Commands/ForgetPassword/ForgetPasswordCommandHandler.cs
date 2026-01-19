using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ExamSystem.Application.Features.Authentication.Commands.ForgetPassword
{
    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppEmailService _emailService;
        private readonly IBackgroundJobService _backgroundJobService;


        public ForgetPasswordCommandHandler(UserManager<ApplicationUser> userManager, IAppEmailService emailService, IBackgroundJobService backgroundJobService)
        {
            _userManager = userManager;
            _emailService = emailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                _backgroundJobService.Enqueue(() =>
                    _emailService.SendEmailForResetPasswordAsync(user, tokenEncoded));
            }
            return Result.Ok("Password reset link sent successfully.");
        }
    }
}
