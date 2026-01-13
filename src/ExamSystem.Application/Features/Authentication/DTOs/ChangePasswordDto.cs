namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record ChangePasswordDto(string CurrentPassword, string NewPassword);
}
