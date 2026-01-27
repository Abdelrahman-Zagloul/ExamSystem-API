using ExamSystem.API.Controllers.V1;
using ExamSystem.API.Tests.Helpers;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests;
using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests;
using ExamSystem.Application.Features.Questions.Queries.GetQuestionById;
using ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId;
using ExamSystem.Application.Features.Questions.Shared;
using ExamSystem.Domain.Entities.Questions;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace ExamSystem.API.Tests.Controllers.V1
{
    [Trait("Category", "API.Controller.V1.Questions")]
    public class QuestionsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly QuestionsController _controller;

        public QuestionsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new QuestionsController(_mediatorMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _controller.HttpContext.Connection.RemoteIpAddress =
                IPAddress.Parse("127.0.0.1");
        }

        [Fact]
        public async Task GetExamQuestions_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");
            var result = Result<PaginatedResult<QuestionResponse>>.Ok(
                PaginatedResult<QuestionResponse>.
                CreatePaginatedResult(new List<QuestionResponse>(), 0, 1, 1, "base url", new Dictionary<string, string>()));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetQuestionsByExamIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamQuestions(1, 1, 10);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<GetQuestionsByExamIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetExamQuestionById_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor"); ;

            var result = Result<QuestionResponse>.Ok(new QuestionResponse());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetQuestionByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamQuestions(1, 5);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<GetQuestionByIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task CreateQuestion_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor"); ;

            var request = new CreateQuestionRequest(
                "Question",
                5,
                QuestionType.MCQ,
                new List<CreateOptionRequest>(),
                3
            );

            var result = Result.Ok("created");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateQuestionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.CreateQuestion(1, request);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<CreateQuestionCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateQuestion_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor"); ;

            var request = new UpdateQuestionRequest(
                "Updated",
                10,
                new List<UpdateOptionRequest>(),
                2
            );

            var result = Result.Ok();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateQuestionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.UpdateQuestion(1, 3, request);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<UpdateQuestionCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteQuestion_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor"); ;
            var result = Result.Ok();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteQuestionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.DeleteQuestion(1, 4);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<DeleteQuestionCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task AnyEndpoint_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor"); ;
            var result = Result.Fail(Error.Forbidden());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<IRequest<Result>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.DeleteQuestion(1, 1);

            // Assert
            response.Should().BeOfType<ObjectResult>();
        }
    }
}
