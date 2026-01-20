using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultDetailsForCurrentStudent;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor;
using ExamSystem.Domain.Constants;
using ExamSystem.Domain.Entities.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamSystem.API.Controllers
{
    public class ExamResultsController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public ExamResultsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("exams/{examId}/results")]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> GetExamResultsForDoctor(int examId, ExamResultStatus? status, int pageNumber = 1, int pageSize = 5)
        {
            var result = await _mediator.Send(new GetExamResultsForDoctorQuery(GetUserId(), examId, status, pageNumber, pageSize, GetBaseUrl(), GetQueryParam()));
            return HandleResult(result);
        }


        [HttpGet("exams/{examId}/my-result")]
        [Authorize(Roles = Role.Student)]
        public async Task<IActionResult> GetExamResultDetails(int examId)
        {
            var result = await _mediator.Send(new GetExamResultDetailsForCurrentStudentQuery(GetUserId(), examId));
            return HandleResult(result);
        }

        [HttpGet("my-results")]
        [Authorize(Roles = Role.Student)]
        public async Task<IActionResult> GetExamResultsForCurrentStudent(int pageNumber = 1, int pageSize = 5)
        {
            var result = await _mediator.Send(new GetExamResultsForCurrentStudentQuery(GetUserId(), pageNumber, pageSize, GetBaseUrl(), GetQueryParam()));
            return HandleResult(result);
        }
    }
}
