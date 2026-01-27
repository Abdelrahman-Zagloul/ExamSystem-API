using ExamSystem.API.Controllers.V1;
using ExamSystem.API.Tests.Helpers;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent.Responses;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor.Responses;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent.Responses;
using ExamSystem.Domain.Entities.Exams;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace ExamSystem.API.Tests.Controllers.V1
{
    [Trait("Category", "API.Controller.V1.ExamResults")]
    public class ExamResultsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ExamResultsController _controller;

        public ExamResultsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ExamResultsController(_mediatorMock.Object);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            _controller.HttpContext.Connection.RemoteIpAddress =
                IPAddress.Parse("127.0.0.1");
        }

        [Fact]
        public async Task GetExamResultsForDoctor_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("doctor-id", "Doctor");
            var result = Result<PaginatedResult<StudentExamResultForDoctorResponse>>.Ok(
                PaginatedResult<StudentExamResultForDoctorResponse>.CreatePaginatedResult(
                    new List<StudentExamResultForDoctorResponse>(), 10, 10, 10, "base-url", new Dictionary<string, string>()));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamResultsForDoctorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamResultsForDoctor(1, ExamResultStatus.Passed, 1, 10);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetExamResultsForDoctorQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetExamResultsForDoctor_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");
            var result = Result<PaginatedResult<StudentExamResultForDoctorResponse>>.Fail(Error.Forbidden());
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamResultsForDoctorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamResultsForDoctor(1, null);

            // Assert
            response.Should().BeOfType<ObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetExamResultsForDoctorQuery>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task GetExamReview_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");
            var result = Result<ExamReviewResponse>.Ok(new ExamReviewResponse());

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamReviewForCurrentStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamReview(2);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetExamReviewForCurrentStudentQuery>(), It.IsAny<CancellationToken>()), Times.Once);

        }

        [Fact]
        public async Task GetExamReview_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");

            var result = Result<ExamReviewResponse>.Fail(Error.NotFound("exam not found", ""));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamReviewForCurrentStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamReview(2);

            // Assert
            response.Should().BeOfType<ObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetExamReviewForCurrentStudentQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task GetExamResultsForCurrentStudent_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");


            var result = Result<PaginatedResult<StudentExamResultForStudentResponse>>.Ok(
                PaginatedResult<StudentExamResultForStudentResponse>.CreatePaginatedResult(
                    new List<StudentExamResultForStudentResponse>(), 10, 10, 10, "base-url", new Dictionary<string, string>()));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamResultsForCurrentStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamResultsForCurrentStudent(1, 5);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetExamResultsForCurrentStudentQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetExamResultsForCurrentStudent_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            _controller.SetUser("student-id", "Student");

            var result = Result<PaginatedResult<StudentExamResultForStudentResponse>>.Fail(Error.BadRequest("invalid paging", ""));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetExamResultsForCurrentStudentQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.GetExamResultsForCurrentStudent();

            // Assert
            response.Should().BeOfType<ObjectResult>();
            _mediatorMock.Verify(m => m.Send(It.IsAny<GetExamResultsForCurrentStudentQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
