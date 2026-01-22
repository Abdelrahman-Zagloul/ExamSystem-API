using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.Shared;
using ExamSystem.Application.Settings;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace ExamSystem.Infrastructure.Identity
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RefreshTokenSettings _settings;
        private readonly IGenericRepository<RefreshToken> _refreshTokenRepo;
        public RefreshTokenService(IUnitOfWork unitOfWork, IOptions<RefreshTokenSettings> options)
        {
            _unitOfWork = unitOfWork;
            _settings = options.Value;
            _refreshTokenRepo = unitOfWork.Repository<RefreshToken>();
        }

        public async Task<RefreshTokenDto> CreateAsync(ApplicationUser user, string? ipAddress, CancellationToken cancellationToken)
        {
            var rawToken = GenerateRefreshToken();
            var tokenHash = HashRefreshToken(rawToken);
            var newRefreshToken = new RefreshToken(user.Id, tokenHash, DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays), ipAddress);

            await _refreshTokenRepo.AddAsync(newRefreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenDto(rawToken, newRefreshToken.ExpiresAt, user.Id);
        }
        public async Task<Result<RefreshTokenDto>> RotateAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var existingToken = await _refreshTokenRepo.GetAsync(x => x.TokenHash == tokenHash, cancellationToken);

            if (existingToken is null)
                return Error.BadRequest("TokenInvalid", "Token is Invalid");

            if (!existingToken.IsActive)
            {
                if (existingToken.ReplacedByTokenId != null)
                    await RevokeAllAsync(existingToken.UserId, ipAddress, cancellationToken);
                return Error.BadRequest("TokenInvalid", "Token is invalid or expired");
            }


            var newRawToken = GenerateRefreshToken();
            var newTokenHash = HashRefreshToken(newRawToken);
            var newRefreshToken = new RefreshToken(existingToken.UserId, newTokenHash, DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays), ipAddress);
            await _refreshTokenRepo.AddAsync(newRefreshToken, cancellationToken);

            existingToken.Revoke(ipAddress, newRefreshToken.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new RefreshTokenDto(newRawToken, newRefreshToken.ExpiresAt, existingToken.UserId);
        }
        public async Task<Result> RevokeAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken)
        {
            var tokenHash = HashRefreshToken(refreshToken);
            var token = await _refreshTokenRepo.GetAsync(x => x.TokenHash == tokenHash, cancellationToken);

            if (token is null || !token.IsActive)
                return Error.BadRequest("TokenInvalid", "Token is invalid or expired");

            token.Revoke(ipAddress);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok("Token Revoked Successfully");
        }
        public async Task RevokeAllAsync(string userId, string? ipAddress, CancellationToken cancellationToken)
        {
            var activeTokens = await _refreshTokenRepo.GetAsQuery(false)
                .Where(x => x.UserId == userId && x.IsActive)
                .ToListAsync(cancellationToken);

            if (activeTokens.Any())
            {
                foreach (var token in activeTokens)
                    token.Revoke(ipAddress);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
        private string HashRefreshToken(string refreshToken)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_settings.RefreshTokenHashKey);
            using var hmac = new HMACSHA256(keyBytes);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(refreshToken)));
        }
    }
}
