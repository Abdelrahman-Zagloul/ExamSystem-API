using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> CreateQuestion(int examId, CreateQuestionCommand command)
        {
            command = command with { ExamId = examId };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpPut("{questionId}")]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> UpdateQuestion(int examId, int questionId, UpdateQuestionCommand command)
        {
            command = command with { ExamId = examId, QuestionId = questionId };
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpDelete("{questionId}")]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> DeleteQuestion(int examId, int questionId)
        {
            var result = await _mediator.Send(new DeleteQuestionCommand(examId, questionId));
            return HandleResult(result);
        }
    }
}
