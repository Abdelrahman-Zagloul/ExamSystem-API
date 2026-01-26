using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Jobs;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.SubmitExam
{
    [Trait("Category", "Application.Exam.SubmitExam.Handler")]

    public class SubmitExamCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<ExamSession>> _sessionRepoMock;
        private readonly Mock<IGenericRepository<StudentAnswer>> _studentAnswerRepoMock;
        private readonly SubmitExamCommandHandler _handler;

        public SubmitExamCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _sessionRepoMock = new Mock<IGenericRepository<ExamSession>>();
            _studentAnswerRepoMock = new Mock<IGenericRepository<StudentAnswer>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<ExamSession>()).Returns(_sessionRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<StudentAnswer>()).Returns(_studentAnswerRepoMock.Object);
            _handler = new SubmitExamCommandHandler(_unitOfWorkMock.Object, _backgroundJobServiceMock.Object);
        }
        private static SubmitExamCommand CreateValidCommand() => new("student-id", 1, new List<SubmitAnswerRequest>
        {
            new SubmitAnswerRequest
            {
                QuestionId = 1,
                SelectedOptionId = 2
            }
        });
        private static Exam CreateValidExam() => new()
        {
            Id = 1,
            EndAt = DateTime.UtcNow.AddMinutes(30),
            DurationInMinutes = 60,
        };
        private static ExamSession CreateValidSession() => new ExamSession(1, "student-id");

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            //Arrange
            var command = CreateValidCommand();

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync((Exam?)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamIsFinished()
        {
            //Arrange
            var command = CreateValidCommand();
            var exam = CreateValidExam();
            exam.EndAt = DateTime.UtcNow.AddMinutes(-1);

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamSessionDoesNotExist()
        {
            //Arrange
            var command = CreateValidCommand();
            var exam = CreateValidExam();

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _sessionRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.StudentId, command.ExamId))
                .ReturnsAsync((ExamSession?)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamAlreadySubmitted()
        {
            //Arrange
            var command = CreateValidCommand();
            var exam = CreateValidExam();
            var session = CreateValidSession();
            session.SubmitSession();

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _sessionRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.StudentId, command.ExamId))
                .ReturnsAsync(session);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamTimeExceeded()
        {
            //Arrange
            var command = CreateValidCommand();
            var exam = CreateValidExam();
            exam.DurationInMinutes = 1;
            var session = new ExamSession(exam.Id, command.StudentId);
            PrivatePropertySetter.Set(session, "StartedAt", DateTime.UtcNow.AddMinutes(-10));

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _sessionRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.StudentId, command.ExamId))
                .ReturnsAsync(session);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldSubmitExamSuccessfully_WhenAllValid()
        {
            var command = CreateValidCommand();
            var exam = CreateValidExam();
            var session = CreateValidSession();

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _sessionRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.StudentId, command.ExamId))
                .ReturnsAsync(session);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            _studentAnswerRepoMock.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<StudentAnswer>>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _backgroundJobServiceMock.Verify(x => x.Enqueue(It.IsAny<Expression<Func<ICalculateExamResultJob, Task>>>()), Times.Once);
        }
    }
}