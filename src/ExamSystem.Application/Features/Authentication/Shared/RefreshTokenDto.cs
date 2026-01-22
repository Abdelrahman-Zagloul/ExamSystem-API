namespace ExamSystem.Application.Features.Authentication.Shared
{
    public record RefreshTokenDto(string RefreshToken, DateTime ExpiresAt, string UserId);
}
