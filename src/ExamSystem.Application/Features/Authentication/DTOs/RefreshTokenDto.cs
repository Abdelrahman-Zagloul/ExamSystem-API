namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record RefreshTokenDto(string RefreshToken, DateTime ExpiresAt, string UserId);
}
