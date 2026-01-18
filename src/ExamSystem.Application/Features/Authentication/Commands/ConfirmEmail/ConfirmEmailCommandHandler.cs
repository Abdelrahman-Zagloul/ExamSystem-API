using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<AuthDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAppEmailService _appEmailService;
        private readonly IBackgroundJobService _backgroundJobService;


        public ConfirmEmailCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService, IAppEmailService appEmailService, IBackgroundJobService backgroundJobService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _appEmailService = appEmailService;
            _backgroundJobService = backgroundJobService;
        }

        public async Task<Result<AuthDto>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Error.NotFound("UserNotFound", $"User with email:'{request.Email}' not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
                return result.Errors.Select(x => Error.Validation(x.Code, x.Description)).ToList();

            _backgroundJobService.Enqueue(() => _appEmailService.SendEmailForWelcomeMessageAsync(user));
            var authDto = await _jwtTokenService.GenerateTokenAsync(user);
            return authDto;
        }
    }
}
