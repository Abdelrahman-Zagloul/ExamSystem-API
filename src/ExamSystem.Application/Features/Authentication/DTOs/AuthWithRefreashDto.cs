namespace ExamSystem.Application.Features.Authentication.DTOs
{
    public record AuthWithRefreshDto(AuthDto AuthDto, RefreshTokenDto RefreshTokenDto);
}
