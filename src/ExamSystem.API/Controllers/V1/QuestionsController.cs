using Asp.Versioning;
using ExamSystem.API.Controllers.Common;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests;
using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests;
using ExamSystem.Application.Features.Questions.Queries.GetQuestionById;
using ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId;
using ExamSystem.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
namespace ExamSystem.API.Controllers.V1
{
    [Authorize(Roles = Role.Doctor)]
    [ApiVersion(1.0)]
    [Route("api/exams/{examId}/questions")]
    [SwaggerTag("Manage exam questions: create, update, delete and list questions for a specific exam.")]
    public class QuestionsController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public QuestionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get exam questions", Description = "Retrieve all questions for a specific exam with pagination support using pageNumber and pageSize query parameters. Accessible only by the exam owner (doctor).")]
        public async Task<IActionResult> GetExamQuestions(int examId, int pageNumber = 1, int pageSize = 10)
        {
            var result = await _mediator.Send(new GetQuestionsByExamIdQuery(GetUserId(), examId, pageNumber, pageSize, GetBaseUrl(), GetQueryParams()));
            return HandleResult(result);
        }

        [HttpGet("{questionId}")]
        [SwaggerOperation(Summary = "Get question by id", Description = "Retrieve question by id for a specific exam. Accessible only by the exam owner (doctor).")]
        public async Task<IActionResult> GetExamQuestions(int examId, int questionId)
        {
            var result = await _mediator.Send(new GetQuestionByIdQuery(GetUserId(), examId, questionId));
            return HandleResult(result);
        }


        [HttpPost]
        [SwaggerOperation(Summary = "Create exam question", Description = "Create a new question for a specific exam. Only the exam owner (doctor) can add questions.")]
        public async Task<IActionResult> CreateQuestion(int examId, CreateQuestionRequest request)
        {
            var result = await _mediator.Send(
                new CreateQuestionCommand(examId, request.QuestionText, request.QuestionMark, request.QuestionType, request.Options, request.CorrectOptionNumber));
            return HandleResult(result);
        }



        [HttpPut("{questionId}")]
        [SwaggerOperation(Summary = "Update exam question", Description = "Update an existing question for a specific exam. Only the exam owner (doctor) can modify questions.")]
        public async Task<IActionResult> UpdateQuestion(int examId, int questionId, UpdateQuestionRequest request)
        {
            var result = await _mediator.Send(
                new UpdateQuestionCommand(examId, questionId, request.QuestionText, request.NewQuestionMark, request.Options, request.NewCorrectOptionId));
            return HandleResult(result);
        }



        [HttpDelete("{questionId}")]
        [SwaggerOperation(Summary = "Delete exam question", Description = "Delete a specific question from an exam. Only the exam owner (doctor) can delete questions.")]
        public async Task<IActionResult> DeleteQuestion(int examId, int questionId)
        {
            var result = await _mediator.Send(new DeleteQuestionCommand(examId, questionId));
            return HandleResult(result);
        }
    }
}
