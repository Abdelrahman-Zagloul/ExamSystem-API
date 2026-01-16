using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.DeleteExam;
using ExamSystem.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Controllers
{
    public class ExamsController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public ExamsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> CreateExam(CreateExamCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpDelete("{examId}")]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> DeleteExam(int examId)
        {
            var result = await _mediator.Send(new DeleteExamCommand(examId));
            return HandleResult(result);
        }


    }
}
