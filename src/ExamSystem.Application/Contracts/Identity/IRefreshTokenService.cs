using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities.Users;

namespace ExamSystem.Application.Contracts.Identity
{
    public interface IRefreshTokenService
    {
        Task<RefreshTokenDto> CreateAsync(ApplicationUser user, string? ipAddress, CancellationToken cancellationToken);
        Task<Result<RefreshTokenDto>> RotateAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken);
        Task<Result> RevokeAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken);
        Task RevokeAllAsync(string userId, string? ipAddress, CancellationToken cancellationToken);
    }
}
