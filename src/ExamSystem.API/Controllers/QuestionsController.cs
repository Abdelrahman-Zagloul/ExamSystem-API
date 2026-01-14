using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Controllers
{
    public class QuestionsController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion(CreateQuestionCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
