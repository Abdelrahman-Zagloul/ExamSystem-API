using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Application.Features.Authentication.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangePasswordCommandHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService, UserManager<ApplicationUser> userManager)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return Error.NotFound("UserNotFound", "User with this id not found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();

            _backgroundJobService.Enqueue(() => _appEmailService.SendEmailForPasswordChangedAsync(user));
            return Result.Ok("Password changed successfully");
        }

    }
}
