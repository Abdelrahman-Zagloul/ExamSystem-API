namespace ExamSystem.Application.Features.Authentication.Commands.ChangePassword.Requests
{
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
}
