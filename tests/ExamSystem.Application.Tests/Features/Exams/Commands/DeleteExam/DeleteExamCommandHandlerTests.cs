using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Exams.Commands.DeleteExam;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.DeleteExam
{

    [Trait("Category", "Application.Exam.Delete.Handler")]
    public class DeleteExamCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<ExamSession>> _examSessionRepoMock;
        private readonly Mock<IGenericRepository<ExamResult>> _examResultRepoMock;
        private readonly DeleteExamCommandHandler _handler;

        public DeleteExamCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _examResultRepoMock = new Mock<IGenericRepository<ExamResult>>();
            _examSessionRepoMock = new Mock<IGenericRepository<ExamSession>>();
            _unitOfWorkMock.Setup(u => u.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<ExamSession>()).Returns(_examSessionRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<ExamResult>()).Returns(_examResultRepoMock.Object);
            _handler = new DeleteExamCommandHandler(_unitOfWorkMock.Object, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            // Arrange
            var command = new DeleteExamCommand(1);
            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                         .ReturnsAsync((Exam?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnForbidden_WhenDoctorIdOfExamNotEqualCurrentUserId()
        {
            // Arrange
            var command = new DeleteExamCommand(1);
            var exam = new Exam
            {
                Id = command.ExamId,
                DoctorId = "DifferentDoctorId"
            };
            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                         .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(s => s.UserId).Returns("CurrentDoctorId");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Forbidden);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamAlreadyFinished()
        {
            // Arrange
            var command = new DeleteExamCommand(1);
            var exam = new Exam
            {
                Id = 1,
                DoctorId = "doctor-id",
                EndAt = DateTime.UtcNow.AddMinutes(-10)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamHasSessions()
        {
            // Arrange
            var command = new DeleteExamCommand(1);
            var exam = new Exam
            {
                Id = 1,
                DoctorId = "doctor-id",
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            _examSessionRepoMock
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamSession, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamHasResults()
        {
            // Arrange
            var command = new DeleteExamCommand(1);
            var exam = new Exam
            {
                Id = 1,
                DoctorId = "doctor-id",
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            _examSessionRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamSession, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _examResultRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamResult, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldDeleteExam_WhenAllConditionsAreMet()
        {
            // Arrange
            var command = new DeleteExamCommand(1);
            var exam = new Exam
            {
                Id = 1,
                DoctorId = "doctor-id",
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);
            _currentUserServiceMock.Setup(x => x.UserId).Returns("doctor-id");
            _examSessionRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamSession, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _examResultRepoMock.Setup(r => r.AnyAsync(It.IsAny<Expression<Func<ExamResult, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _examRepoMock.Verify(r => r.Remove(exam), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


    }
}
