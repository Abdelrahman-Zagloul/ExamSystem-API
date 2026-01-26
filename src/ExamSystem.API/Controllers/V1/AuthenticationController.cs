using Asp.Versioning;
using ExamSystem.API.Controllers.Common;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Features.Authentication.Commands.ChangePassword;
using ExamSystem.Application.Features.Authentication.Commands.ChangePassword.Requests;
using ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ForgetPassword;
using ExamSystem.Application.Features.Authentication.Commands.Login;
using ExamSystem.Application.Features.Authentication.Commands.Login.Requests;
using ExamSystem.Application.Features.Authentication.Commands.Logout;
using ExamSystem.Application.Features.Authentication.Commands.RefreshToken;
using ExamSystem.Application.Features.Authentication.Commands.Register;
using ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ResetPassword;
using ExamSystem.Application.Features.Authentication.Commands.RevokeToken;
using ExamSystem.Application.Features.Authentication.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExamSystem.API.Controllers.V1
{
    [Route("api/auth")]
    [ApiVersion(1.0)]
    [SwaggerTag("Manage user authentication: register, login, logout, confirm and resend confirm email, refresh token, revoke token, forget, reset and change password")]
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
        [SwaggerOperation(Summary = "User login", Description = "Authenticate user using email and password. Returns access token and sets refresh token in HTTP-only cookie.")]
        public async Task<IActionResult> LoginAsync(LoginRequest request)
        {
            var result = await _mediator.Send(new LoginCommand(request.Email, request.Password, IpAddress));
            if (!result.IsSuccess)
                return HandleResult(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenDto.RefreshToken, result.Value.RefreshTokenDto.ExpiresAt);
            return HandleResult(Result<AccessTokenResponse>.Ok(result.Value.AccessTokenResponse, "Login Successfully"));
        }


        [HttpPost("register")]
        [SwaggerOperation(Summary = "User registration", Description = "Register a new user and send email confirmation link.")]
        public async Task<IActionResult> RegisterAsync(RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("logout")]
        [SwaggerOperation(Summary = "User logout", Description = "Logout the currently authenticated user and revoke all refresh token.")]
        public async Task<IActionResult> LogoutAsync()
        {
            var result = await _mediator.Send(new LogoutCommand(GetUserId(), IpAddress));
            RemoveRefreshTokenFromCookie();
            return HandleResult(result);
        }


        [HttpGet("confirm-email")]
        [SwaggerOperation(Summary = "Confirm email", Description = "Confirm user email using confirmation token. On success, the user is automatically authenticated and an access token is returned.")]
        public async Task<IActionResult> ConfirmEmailAsync(string email, string token)
        {
            var result = await _mediator.Send(new ConfirmEmailCommand(email, token, IpAddress));
            if (!result.IsSuccess)
                return HandleResult(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenDto.RefreshToken, result.Value.RefreshTokenDto.ExpiresAt);
            return HandleResult(Result<AccessTokenResponse>.Ok(result.Value.AccessTokenResponse, "Email confirmed successfully"));
        }


        [HttpPost("email/resend-confirmation")]
        [SwaggerOperation(Summary = "Resend confirmation email", Description = "Resend email confirmation link to the user.")]
        public async Task<IActionResult> ResendConfirmEmailAsync(ResendConfirmEmailCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpPost("token/refresh")]
        [SwaggerOperation(Summary = "Refresh access token", Description = "Generate a new access token using refresh token stored in HTTP-only cookie.")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken, IpAddress));
            if (!result.IsSuccess)
                return HandleResult(result);

            AddRefreshTokenToCookie(result.Value.RefreshTokenDto.RefreshToken, result.Value.RefreshTokenDto.ExpiresAt);
            return HandleResult(Result<AccessTokenResponse>.Ok(result.Value.AccessTokenResponse, "Token refreshed successfully"));
        }


        [HttpPost("token/revoke")]
        [SwaggerOperation(Summary = "Revoke refresh token", Description = "Revoke the refresh token for the currently authenticated user.")]
        public async Task<IActionResult> RevokeTokenAsync()
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            var result = await _mediator.Send(new RevokeTokenCommand(refreshToken, IpAddress));
            return HandleResult(result);
        }


        [Authorize]
        [HttpPost("password/change")]
        [SwaggerOperation(Summary = "Change password", Description = "Change password for the currently authenticated user.")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest dto)
        {
            var result = await _mediator.Send(new ChangePasswordCommand(GetUserId(), dto.CurrentPassword, dto.NewPassword));
            return HandleResult(result);
        }


        [HttpPost("password/forgot")]
        [SwaggerOperation(Summary = "Forgot password", Description = "Send password reset link to user's email.")]
        public async Task<IActionResult> ForgetPasswordAsync(ForgetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpPost("password/reset")]
        [SwaggerOperation(Summary = "Reset password", Description = "Reset user password using reset token sent to email.")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
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
