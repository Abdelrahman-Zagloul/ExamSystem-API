namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record JwtTokenDto(string AccessToken, DateTime ExpiresAt);
}
