using ExamSystem.Application.Features.Authentication.Commands.ChangePassword;
using ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ForgetPassword;
using ExamSystem.Application.Features.Authentication.Commands.Login;
using ExamSystem.Application.Features.Authentication.Commands.Register;
using ExamSystem.Application.Features.Authentication.Commands.ResetPassword;
using ExamSystem.Application.Features.Authentication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IMediator _mediator;
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

        [HttpGet("forget-password")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var result = await _mediator.Send(new ForgetPasswordCommand(email));
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
    }
}
