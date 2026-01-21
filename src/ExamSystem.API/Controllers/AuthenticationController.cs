using ExamSystem.Application.Features.Authentication.Commands.ChangePassword;
using ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ForgetPassword;
using ExamSystem.Application.Features.Authentication.Commands.Login;
using ExamSystem.Application.Features.Authentication.Commands.Logout;
using ExamSystem.Application.Features.Authentication.Commands.RefreshToken;
using ExamSystem.Application.Features.Authentication.Commands.Register;
using ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ResetPassword;
using ExamSystem.Application.Features.Authentication.Commands.RevokeToken;
using ExamSystem.Application.Features.Authentication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private string? IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
        private const string RefreshTokenCookieName = "refreshToken";
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(email, token));
            return HandleResult(result);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var result = await _mediator.Send(new ChangePasswordCommand(GetUserId() ?? "", dto.CurrentPassword, dto.NewPassword));
            return HandleResult(result);
        }

        [HttpPost("resend-confirm-email")]
        public async Task<IActionResult> ChangePassword(ResendConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken, IpAddress));
            if (!result.IsSuccess)
                return HandleResult(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenDto.RefreshToken, result.Value.RefreshTokenDto.ExpiresAt);
            return Ok(result.Value.AuthDto);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _mediator.Send(new RevokeTokenCommand(refreshToken, IpAddress));
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand(GetUserId(), IpAddress));
            RemoveRefreshTokenFromCookie();
            return HandleResult(result);
        }

        private void AddRefreshTokenToCookie(string refreshToken, DateTime expires)
        {
            Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expires
            });
        }
        private void RemoveRefreshTokenFromCookie()
        {
            Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }

    }
}
