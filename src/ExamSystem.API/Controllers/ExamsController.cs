using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.DeleteExam;
using ExamSystem.Application.Features.Exams.Commands.StartExam;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam.Requests;
using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent;
using ExamSystem.Domain.Constants;
using ExamSystem.Domain.Entities.Exams;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ExamSystem.API.Controllers
{
    [Route("api/exams")]
    [SwaggerTag("Manage exams: create, update, delete, view exams for doctors and students with different response, start and submit exams.")]
    public class ExamsController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public ExamsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("{examId}")]
        [Authorize(Roles = Role.Doctor)]
        [SwaggerOperation(Summary = "Get exam by id", Description = "Retrieve full exam details by exam id. Accessible only by the exam owner (doctor).")]
        public async Task<IActionResult> GetExamByIdForDoctor(int examId)
        {
            var result = await _mediator.Send(new GetExamByIdForDoctorQuery(GetUserId()!, examId));
            return HandleResult(result);
        }


        [HttpGet("doctor")]
        [Authorize(Roles = Role.Doctor)]
        [SwaggerOperation(Summary = "Get exams for doctor", Description = "Retrieve all exams created by the current doctor with optional status filter and pagination using examStatus, pageNumber and pageSize.")]
        public async Task<IActionResult> GetExamsForDoctor(ExamStatus? examStatus, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetExamsForDoctorQuery(GetUserId()!, examStatus, pageNumber, pageSize, GetBaseUrl(), GetQueryParams()));
            return HandleResult(result);
        }


        [HttpGet("student")]
        [Authorize(Roles = Role.Student)]
        [SwaggerOperation(Summary = "Get exams for student", Description = "Retrieve all available exams for the current student with optional status filter and pagination using examStatus, pageNumber and pageSize.")]
        public async Task<IActionResult> GetExamsForStudent(ExamStatus? examStatus, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetExamsForStudentQuery(examStatus, pageNumber, pageSize, GetBaseUrl(), GetQueryParams()));
            return HandleResult(result);
        }



        [HttpPost("{examId}/start")]
        [Authorize(Roles = Role.Student)]
        [SwaggerOperation(Summary = "Start exam", Description = "Start the exam for the current student and create an exam session.")]
        public async Task<IActionResult> StartExam(int examId)
        {
            var result = await _mediator.Send(new StartExamCommand(GetUserId()!, examId));
            return HandleResult(result);
        }


        [HttpPost("{examId}/submit")]
        [Authorize(Roles = Role.Student)]
        [SwaggerOperation(Summary = "Submit exam", Description = "Submit student answers for a specific exam and finalize the exam session.")]
        public async Task<IActionResult> SubmitExam(int examId, List<SubmitAnswerRequest> answers)
        {
            var result = await _mediator.Send(new SubmitExamCommand(GetUserId()!, examId, answers));
            return HandleResult(result);
        }



        [HttpPost]
        [Authorize(Roles = Role.Doctor)]
        [SwaggerOperation(Summary = "Create exam", Description = "Create a new exam. Only doctors are allowed to create exams.")]
        public async Task<IActionResult> CreateExam(CreateExamCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }


        [HttpPut("{examId}")]
        [Authorize(Roles = Role.Doctor)]
        [SwaggerOperation(Summary = "Update exam", Description = "Update exam details such as title, description, dates and duration. Only the exam owner (doctor) can update the exam.")]
        public async Task<IActionResult> UpdateExam(int examId, [FromBody] UpdateExamRequest request)
        {
            var result = await _mediator.Send(new UpdateExamCommand(examId, request.Title, request.Description, request.StartAt, request.EndAt, request.DurationInMinutes));
            return HandleResult(result);
        }


        [HttpDelete("{examId}")]
        [Authorize(Roles = Role.Doctor)]
        [SwaggerOperation(Summary = "Delete exam", Description = "Delete a specific exam. Only the exam owner (doctor) can delete the exam.")]
        public async Task<IActionResult> DeleteExam(int examId)
        {
            var result = await _mediator.Send(new DeleteExamCommand(examId));
            return HandleResult(result);
        }


    }
}
