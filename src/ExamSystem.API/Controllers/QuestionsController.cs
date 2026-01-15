using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Controllers
{
    [Route("api/exams/{examId}/questions")]
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

        [HttpDelete("{questionId}")]
        public async Task<IActionResult> DeleteQuestion(int examId, int questionId)
        {
            var result = await _mediator.Send(new DeleteQuestionCommand(examId, questionId));
            return HandleResult(result);
        }
    }
}
