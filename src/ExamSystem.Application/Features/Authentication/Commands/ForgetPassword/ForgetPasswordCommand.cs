using ExamSystem.Application.Common.Results;

namespace ExamSystem.Application.Features.Authentication.Commands.ForgetPassword
{
    public record ForgetPasswordCommand(string Email) : IResultRequest<string>;
}
