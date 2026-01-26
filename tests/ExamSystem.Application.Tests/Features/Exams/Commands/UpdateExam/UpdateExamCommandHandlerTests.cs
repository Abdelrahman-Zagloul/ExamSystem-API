using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using ExamSystem.Application.Features.Exams.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.UpdateExam
{
    [Trait("Category", "Application.Exam.Update.Handler")]
    public class UpdateExamCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly UpdateExamCommandHandler _handler;
        private static UpdateExamCommand CreateEmptyCommand() =>
            new UpdateExamCommand(1, null, null, null, null, null);
        public UpdateExamCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<ExamMappingProfile>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _handler = new UpdateExamCommandHandler(_unitOfWorkMock.Object, _mapper, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExists()
        {
            // Arrange
            var command = CreateEmptyCommand();
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
            var command = CreateEmptyCommand();
            var exam = new Exam
            {
                Id = command.ExamId,
                DoctorId = "another-doctor",
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("current-doctor");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Forbidden);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExamAlreadyStarted()
        {
            // Arrange
            var command = CreateEmptyCommand();
            var exam = new Exam
            {
                Id = command.ExamId,
                DoctorId = "doctor-id",
                StartAt = DateTime.UtcNow.AddMinutes(-10),
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.BadRequest);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenEndAtIsLessThanOrEqualStartAt()
        {
            // Arrange
            var command = CreateEmptyCommand() with
            {
                StartAt = DateTime.UtcNow.AddDays(2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            var exam = new Exam
            {
                Id = command.ExamId,
                DoctorId = "doctor-id",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(3)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.BadRequest);
        }

        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenDurationExceedsExamTimeRange()
        {
            // Arrange
            var command = CreateEmptyCommand() with
            {
                StartAt = DateTime.UtcNow.AddMinutes(10),
                EndAt = DateTime.UtcNow.AddMinutes(20),
                DurationInMinutes = 30
            };

            var exam = new Exam
            {
                Id = command.ExamId,
                DoctorId = "doctor-id",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(2)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.BadRequest);
        }

        [Fact]
        public async Task Handle_ShouldUpdateExam_WhenCommandIsValid()
        {
            // Arrange
            var command = CreateEmptyCommand() with
            {
                Title = "Updated Title"
            };

            var exam = new Exam
            {
                Id = command.ExamId,
                DoctorId = "doctor-id",
                StartAt = DateTime.UtcNow.AddDays(1),
                EndAt = DateTime.UtcNow.AddDays(2)
            };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _currentUserServiceMock.Setup(x => x.UserId)
                .Returns("doctor-id");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
