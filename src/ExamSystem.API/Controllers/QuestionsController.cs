using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Application.Features.Questions.DTOs;
using ExamSystem.Application.Features.Questions.Queries.GetAllQuestionsForDoctor;
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

        [HttpGet]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> GetExamQuestions(int examId, int pageNumber = 1, int pageSize = 5)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}";
            var result = await _mediator.Send(new GetExamQuestionsForDoctorQuery(GetUserId()!, examId, pageNumber, pageSize, baseUrl));
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> CreateQuestion(int examId, CreateQuestionRequestDto dto)
        {
            var result = await _mediator.Send(
                new CreateQuestionCommand(examId, dto.QuestionText, dto.QuestionMark, dto.QuestionType, dto.Options, dto.CorrectOptionNumber));
            return HandleResult(result);
        }


        [HttpPut("{questionId}")]
        [Authorize(Roles = Role.Doctor)]
        public async Task<IActionResult> UpdateQuestion(int examId, int questionId, UpdateQuestionRequestDto dto)
        {
            var result = await _mediator.Send(
                new UpdateQuestionCommand(examId, questionId, dto.QuestionText, dto.NewQuestionMark, dto.Options, dto.NewCorrectOptionId));
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
