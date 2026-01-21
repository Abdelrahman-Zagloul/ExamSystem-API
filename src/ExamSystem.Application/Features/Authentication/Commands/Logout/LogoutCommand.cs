using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.Logout
{
    public record LogoutCommand(string UserId, string? IpAddress) : IRequest<Result>;
}
