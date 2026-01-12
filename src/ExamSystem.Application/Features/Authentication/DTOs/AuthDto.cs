namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record AuthDto(
        string AccessToken,
        string Role,
        string UserId,
        DateTime ExpiresAt
    );
}
