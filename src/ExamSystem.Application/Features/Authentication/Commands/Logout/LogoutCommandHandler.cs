using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Contracts.Identity;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IRefreshTokenService _refreshTokenService;

        public LogoutCommandHandler(IRefreshTokenService refreshTokenService)
        {
            _refreshTokenService = refreshTokenService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _refreshTokenService.RevokeAllAsync(request.UserId, request.IpAddress, cancellationToken);
            return Result.Ok("Logged out successfully");
        }
    }
}
