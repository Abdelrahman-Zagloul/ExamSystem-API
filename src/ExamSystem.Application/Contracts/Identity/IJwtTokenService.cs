using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Domain.Entities;

namespace ExamSystem.Application.Contracts.Identity
{
    public interface IJwtTokenService
    {
        Task<AuthDto> GenerateTokenAsync(ApplicationUser user);
    }
}
