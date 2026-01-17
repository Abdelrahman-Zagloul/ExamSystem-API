using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.DeleteExam;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Application.Features.Exams.DTOs;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor;
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


        [HttpGet]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> CreateExam(ExamStatus? examStatus, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _mediator.Send(new GetExamsForDoctorQuery(GetUserId()!, examStatus, pageNumber, pageSize, GetBaseUrl()));
            return HandleResult(result);
        }


        [HttpPost]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> CreateExam(CreateExamCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpPut("{examId}")]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> UpdateExam(int examId, [FromBody] UpdateExamRequestDto dto)
        {
            var result = await _mediator.Send(new UpdateExamCommand(examId, dto.Title, dto.Description, dto.StartAt, dto.EndAt, dto.DurationInMinutes));
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
