using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.Shared;
using ExamSystem.Domain.Entities.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace ExamSystem.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AccessWithRefreshTokenDto>>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAccessTokenService _accessTokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RefreshTokenCommandHandler(IRefreshTokenService refreshTokenService, IAccessTokenService accessTokenService, UserManager<ApplicationUser> userManager)
        {
            _refreshTokenService = refreshTokenService;
            _accessTokenService = accessTokenService;
            _userManager = userManager;
        }

        public async Task<Result<AccessWithRefreshTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Error.Unauthorized("InvalidCredentials", "Invalid authentication credentials");

            var refreshResultDto = await _refreshTokenService.RotateAsync(request.RefreshToken, request.IpAddress, cancellationToken);
            if (!refreshResultDto.IsSuccess)
                return refreshResultDto.Errors[0];

            var user = await _userManager.FindByIdAsync(refreshResultDto.Value.UserId);
            if (user == null)
                return Error.NotFound("UserNotFound", "user with this id not exist");

            var authDto = await _accessTokenService.GenerateTokenAsync(user);

            return new AccessWithRefreshTokenDto(authDto, refreshResultDto.Value);
        }
    }
}
