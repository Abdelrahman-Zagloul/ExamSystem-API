using ExamSystem.API.Controllers.V1;
using ExamSystem.API.Tests.Helpers;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Commands.DeleteExam;
using ExamSystem.Application.Features.Exams.Commands.StartExam;
using ExamSystem.Application.Features.Exams.Commands.StartExam.Responses;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam.Requests;
using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor;
using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor.Responses;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent.Responses;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace ExamSystem.API.Tests.Controllers.V1
{
    [Trait("Category", "API.Controller.V1.Exams")]
    public class ExamsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ExamsController _controller;

        public ExamsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ExamsController(_mediatorMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.HttpContext.Connection.RemoteIpAddress =
                IPAddress.Parse("127.0.0.1");
        }

        [Fact]
        public async Task GetExamByIdForDoctor_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");
            var result = Result<ExamDetailsForDoctorResponse>.Ok(new ExamDetailsForDoctorResponse());
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamByIdForDoctorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamByIdForDoctor(1);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(
                m => m.Send(It.IsAny<GetExamByIdForDoctorQuery>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetExamsForDoctor_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");

            var result = Result<PaginatedResult<ExamSummaryResponse>>.Ok(
                PaginatedResult<ExamSummaryResponse>.
                CreatePaginatedResult(new List<ExamSummaryResponse>(), 0, 1, 1, "base url", new Dictionary<string, string>()));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamsForDoctorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamsForDoctor(null, 1, 2);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task CreateExam_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");

            var command = new CreateExamCommand("title", "description", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 10);
            var result = Result.Ok("created");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.CreateExam(command);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateExam_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");
            var request = new UpdateExamRequest("title", "desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 60);
            var result = Result.Ok();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateExamCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.UpdateExam(1, request);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteExam_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");

            var result = Result.Ok();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<DeleteExamCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.DeleteExam(1);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetExamsForStudent_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");

            var result = Result<PaginatedResult<ExamDetailsForStudentResponse>>.Ok(
                 PaginatedResult<ExamDetailsForStudentResponse>.
                CreatePaginatedResult(new List<ExamDetailsForStudentResponse>(), 0, 1, 1, "base url", new Dictionary<string, string>()));


            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamsForStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamsForStudent(null, 1, 2);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task StartExam_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");

            var result = Result<StartExamResponse>.Ok(new StartExamResponse());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<StartExamCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.StartExam(1);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SubmitExam_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");

            var answers = new List<SubmitAnswerRequest>();
            var result = Result.Ok("submitted");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SubmitExamCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.SubmitExam(1, answers);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AnyEndpoint_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");

            var result = Result.Fail(Error.Forbidden());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<IRequest<Result>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.DeleteExam(1);

            // Assert
            response.Should().BeOfType<ObjectResult>();
        }
    }
}
