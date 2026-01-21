using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result>
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public RevokeTokenCommandHandler(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Error.Unauthorized("InvalidCredentials", "Invalid authentication credentials");

            return await _refreshTokenService
                .RevokeAsync(request.RefreshToken, request.IpAddress, cancellationToken);
        }
    }
}
