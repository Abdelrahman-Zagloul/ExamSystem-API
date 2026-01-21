using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.RevokeToken
{
    public record RevokeTokenCommand(string? RefreshToken, string? IpAddress) : IRequest<Result>;
}
