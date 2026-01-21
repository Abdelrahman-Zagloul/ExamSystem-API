using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Features.Authentication.DTOs;

namespace ExamSystem.Application.Features.Authentication.Commands.RefreshToken
{
    public record RefreshTokenCommand(string? RefreshToken, string? IpAddress)
        : IResultRequest<AuthWithRefreshDto>;
}
