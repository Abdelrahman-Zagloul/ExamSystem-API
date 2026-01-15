using ExamSystem.Application.Common.Results;
using MediatR;

namespace ExamSystem.Application.Features.Authentication.Commands.ChangePassword
{
    public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword)
        : IRequest<Result>;
}
