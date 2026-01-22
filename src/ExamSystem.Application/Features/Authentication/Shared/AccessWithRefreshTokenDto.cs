namespace ExamSystem.Application.Features.Authentication.Shared
{
    public record AccessWithRefreshTokenDto(AccessTokenResponse AccessTokenResponse, RefreshTokenDto RefreshTokenDto);
}
