namespace ExamSystem.Application.Features.Authentication.Shared
{
    public record AccessTokenResponse(
        string AccessToken,
        string Role,
        string UserId,
        DateTime ExpiresAt
    );
}
