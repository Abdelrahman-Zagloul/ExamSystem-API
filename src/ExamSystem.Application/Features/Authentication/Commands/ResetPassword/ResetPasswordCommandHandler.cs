using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ExamSystem.Application.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<string>>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordCommandHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService, UserManager<ApplicationUser> userManager)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
            _userManager = userManager;
        }

        public async Task<Result<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<string>.Fail(Error.NotFound("UserNotFound", "User with this email does not exist."));

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var resetPassResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (!resetPassResult.Succeeded)
                return Result<string>.Fail(resetPassResult.Errors.Select(x => Error.Validation(x.Code, x.Description)).ToList());

            _backgroundJobService.Enqueue(() => _appEmailService.SendEmailForPasswordChangedAsync(user));
            return Result<string>.Ok("Password reset successfully");
        }
    }
}
