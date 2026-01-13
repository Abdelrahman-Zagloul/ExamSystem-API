using ExamSystem.Application.Common.Results;

namespace ExamSystem.Application.Features.Authentication.Commands.ChangePassword
{
    public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword)
        : IResultRequest<string>;
}
