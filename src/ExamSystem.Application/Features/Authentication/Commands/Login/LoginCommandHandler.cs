using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Application.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AccessWithRefreshTokenDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccessTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IAccessTokenService jwtTokenService, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<Result<AccessWithRefreshTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Error.Unauthorized("InvalidCredentials", "Email or password is invalid.");

            if (!user.EmailConfirmed)
                return Error.Unauthorized("EmailNotConfirmed", "Please confirm your email before logging in.");

            var accessTokenDto = await _jwtTokenService.GenerateTokenAsync(user);
            var refreshTokenDto = await _refreshTokenService.CreateAsync(user, request.IpAddress, cancellationToken);

            return new AccessWithRefreshTokenDto(accessTokenDto, refreshTokenDto);
        }
    }
}
