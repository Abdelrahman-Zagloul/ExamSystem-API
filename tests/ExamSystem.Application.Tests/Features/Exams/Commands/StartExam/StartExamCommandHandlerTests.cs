using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Features.Exams.Commands.StartExam;
using ExamSystem.Application.Features.Exams.Commands.StartExam.Responses;
using ExamSystem.Application.Tests.Features.Exams.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.StartExam
{
    [Trait("Category", "Application.Exam.StartExam.Handler")]
    public class StartExamCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<ExamSession>> _examSessionRepoMock;
        private readonly StartExamCommandHandler _handler;
        public StartExamCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<TestExamMappingProfile>();
            _cacheServiceMock = new Mock<ICacheService>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _examSessionRepoMock = new Mock<IGenericRepository<ExamSession>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<ExamSession>()).Returns(_examSessionRepoMock.Object);
            _handler = new StartExamCommandHandler(_unitOfWorkMock.Object, _mapper, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExists()
        {
            //Arrange
            var command = new StartExamCommand("student-id", 1);
            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync((Exam?)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamNotStartedYet()
        {
            // Arrange
            var exam = new Exam
            {
                Id = 1,
                StartAt = DateTime.UtcNow.AddMinutes(10),
                EndAt = DateTime.UtcNow.AddHours(1)
            };
            var command = new StartExamCommand("student-id", exam.Id);


            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), exam.Id))
                .ReturnsAsync(exam);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamAlreadyFinished()
        {
            // Arrange
            var exam = new Exam
            {
                Id = 1,
                StartAt = DateTime.UtcNow.AddHours(-2),
                EndAt = DateTime.UtcNow.AddMinutes(-1)
            };
            var command = new StartExamCommand("student-id", exam.Id);

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), exam.Id))
                .ReturnsAsync(exam);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamAlreadySubmitted()
        {
            // Arrange
            var exam = new Exam
            {
                Id = 1,
                StartAt = DateTime.UtcNow.AddHours(-2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };
            var session = new ExamSession(exam.Id, "student-id");
            session.SubmitSession();
            var command = new StartExamCommand("student-id", exam.Id);

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), exam.Id))
                .ReturnsAsync(exam);

            _examSessionRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), "student-id", exam.Id))
                .ReturnsAsync(session);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnExamResponse_WhenSessionExistsAndNotSubmitted()
        {
            // Arrange
            var exam = new Exam
            {
                Id = 1,
                StartAt = DateTime.UtcNow.AddHours(-2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };
            var session = new ExamSession(exam.Id, "student-id");
            var command = new StartExamCommand("student-id", exam.Id);

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), exam.Id))
                .ReturnsAsync(exam);

            _examSessionRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), "student-id", exam.Id))
                .ReturnsAsync(session);

            _cacheServiceMock.Setup(x => x.GetAsync<StartExamResponse>(It.IsAny<string>()))
                .ReturnsAsync(new StartExamResponse
                {
                    ExamId = exam.Id,
                    Questions = new List<ExamQuestionResponse>()
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ShouldCreateSession_WhenSessionDoesNotExist()
        {
            // Arrange
            var exam = new Exam
            {
                Id = 1,
                StartAt = DateTime.UtcNow.AddHours(-2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };
            var command = new StartExamCommand("student-id", exam.Id);

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), exam.Id))
                .ReturnsAsync(exam);

            _examSessionRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), "student-id", exam.Id))
                .ReturnsAsync((ExamSession?)null);

            _cacheServiceMock.Setup(x => x.GetAsync<StartExamResponse>(It.IsAny<string>()))
                .ReturnsAsync(new StartExamResponse
                {
                    ExamId = exam.Id,
                    Questions = new List<ExamQuestionResponse>()
                });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _examSessionRepoMock.Verify(x => x.AddAsync(It.IsAny<ExamSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}