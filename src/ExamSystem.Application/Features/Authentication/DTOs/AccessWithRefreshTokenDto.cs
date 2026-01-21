namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record AccessWithRefreshTokenDto(AccessTokenDto AccessTokenDto, RefreshTokenDto RefreshTokenDto);
}
