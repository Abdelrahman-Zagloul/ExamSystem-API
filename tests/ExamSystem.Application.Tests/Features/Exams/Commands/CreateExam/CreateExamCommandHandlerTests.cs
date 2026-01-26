using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using ExamSystem.Application.Features.Exams.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.CreateExam
{
    [Trait("Category", "Application.Exam.Create.Handler")]
    public class CreateExamCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly Mock<ICurrentUserService> _currentUserMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<Doctor>> _doctorRepoMock;
        private readonly CreateExamCommandHandler _handler;
        public CreateExamCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<ExamMappingProfile>();
            _currentUserMock = new Mock<ICurrentUserService>();
            _handler = new CreateExamCommandHandler(_unitOfWorkMock.Object, _mapper, _currentUserMock.Object);
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _doctorRepoMock = new Mock<IGenericRepository<Doctor>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<Doctor>()).Returns(_doctorRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenDoctorDoesNotExist()
        {
            // Arrange
            var command = new CreateExamCommand("Title", "Description", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 60);

            _currentUserMock.Setup(x => x.UserId).Returns("diff-doctor-id");
            _doctorRepoMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);
            // Assert

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.Unauthorized);
        }

        [Fact]
        public async Task Handle_ShouldCreateExam_WhenCommandIsValid()
        {
            // Arrange
            var command = new CreateExamCommand("Title", "Description", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 60);
            _currentUserMock.Setup(x => x.UserId).Returns("doctor-id");
            _doctorRepoMock.Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Doctor, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var exam = _mapper.Map<Exam>(command);
            exam.DoctorId = "doctor-id";

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _examRepoMock.Verify(r => r.AddAsync(It.Is<Exam>(e => e.Title == command.Title && e.DoctorId == "doctor-id")
                , It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

