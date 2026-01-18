using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public class ResendConfirmEmailCommandHandler : IRequestHandler<ResendConfirmEmailCommand, Result>
    {
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResendConfirmEmailCommandHandler(IAppEmailService appEmailService, IBackgroundJobService backgroundJobService, UserManager<ApplicationUser> userManager)
        {
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
            _userManager = userManager;
        }

        public async Task<Result> Handle(ResendConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                if (user.EmailConfirmed == true)
                    return Error.Conflict("EmailAlreadyConfirmed", "This Email is already confirmed.");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                _backgroundJobService.Enqueue(() => _appEmailService.SendEmailForConfirmEmailAsync(user, encodedToken));
            }
            return Result.Ok("Confirmation email has been sent.");
        }
    }
}
