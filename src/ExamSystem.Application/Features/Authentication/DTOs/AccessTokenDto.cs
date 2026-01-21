namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record AccessTokenDto(
        string AccessToken,
        string Role,
        string UserId,
        DateTime ExpiresAt
    );
}
