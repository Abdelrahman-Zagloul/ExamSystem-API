using ExamSystem.Application.Features.Authentication.Commands.Login;
using MediatR;
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

        [HttpGet("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

    }
}
