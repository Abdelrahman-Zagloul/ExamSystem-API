using ExamSystem.Application.Common.Results;

namespace ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail
{
    public record ResendConfirmEmailCommand(string Email) : IResultRequest<string>;
}
