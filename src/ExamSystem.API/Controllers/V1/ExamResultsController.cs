using Asp.Versioning;
using ExamSystem.API.Controllers.Common;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent;
using ExamSystem.Domain.Constants;
using ExamSystem.Domain.Entities.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExamSystem.API.Controllers.V1
{
    [Route("api")]
    [ApiVersion(1.0)]
    [SwaggerTag("Manage exam results and reviews: view exam results for doctors, review exams for students, and list student results.")]
    public class ExamResultsController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public ExamResultsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = Role.Doctor)]
        [HttpGet("exams/{examId}/results")]
        [SwaggerOperation(Summary = "Get exam results for doctor", Description = "Retrieve all exam results for a specific exam with optional status filter and pagination using status, pageNumber and pageSize. Accessible only by the exam owner (doctor).")]
        public async Task<IActionResult> GetExamResultsForDoctor(int examId, ExamResultStatus? status, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetExamResultsForDoctorQuery(GetUserId(), examId, status, pageNumber, pageSize, GetBaseUrl(), GetQueryParams()));
            return HandleResult(result);
        }




        [Authorize(Roles = Role.Student)]
        [HttpGet("exams/{examId}/review")]
        [SwaggerOperation(Summary = "Get exam review", Description = "Retrieve the exam review for the current student including answers, correct answers, scores and final result after the exam is completed.")]
        public async Task<IActionResult> GetExamReview(int examId)
        {
            var result = await _mediator.Send(new GetExamReviewForCurrentStudentQuery(GetUserId(), examId));
            return HandleResult(result);
        }



        [Authorize(Roles = Role.Student)]
        [HttpGet("results/me")]
        [SwaggerOperation(Summary = "Get my exam results", Description = "Retrieve all exam results for the current student with pagination support using pageNumber and pageSize.")]
        public async Task<IActionResult> GetExamResultsForCurrentStudent(int pageNumber = 1, int pageSize = 5)
        {
            var result = await _mediator.Send(new GetExamResultsForCurrentStudentQuery(GetUserId(), pageNumber, pageSize, GetBaseUrl(), GetQueryParams()));
            return HandleResult(result);
        }
    }
}
