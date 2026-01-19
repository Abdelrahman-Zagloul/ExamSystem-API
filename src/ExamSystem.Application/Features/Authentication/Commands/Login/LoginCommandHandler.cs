using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Application.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<Result<AuthDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Error.Unauthorized("InvalidCredentials", "Email or password is invalid.");

            if (!user.EmailConfirmed)
                return Error.Unauthorized("EmailNotConfirmed", "Please confirm your email before logging in.");

            var authDto = await _jwtTokenService.GenerateTokenAsync(user);
            return authDto;
        }
    }
}
