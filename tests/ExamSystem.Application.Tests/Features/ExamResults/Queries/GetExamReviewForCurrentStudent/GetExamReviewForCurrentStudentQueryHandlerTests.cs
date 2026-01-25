using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent;
using ExamSystem.Application.Tests.Features.ExamResults.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.ExamResults.Queries.GetExamReviewForCurrentStudent
{
    [Trait("Category", "Application.Question.ExamReview.Handler")]
    public class GetExamReviewForCurrentStudentQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<ExamResult>> _examResultRepoMock;
        private readonly Mock<IGenericRepository<StudentAnswer>> _studentAnswerRepoMock;
        private readonly GetExamReviewForCurrentStudentQueryHandler _handler;
        public GetExamReviewForCurrentStudentQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<TestExamResultMappingProfile>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _examResultRepoMock = new Mock<IGenericRepository<ExamResult>>();
            _studentAnswerRepoMock = new Mock<IGenericRepository<StudentAnswer>>();
            _unitOfWorkMock.Setup(u => u.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<ExamResult>()).Returns(_examResultRepoMock.Object);
            _unitOfWorkMock.Setup(u => u.Repository<StudentAnswer>()).Returns(_studentAnswerRepoMock.Object);
            _handler = new GetExamReviewForCurrentStudentQueryHandler(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            //Arrange
            var query = new GetExamReviewForCurrentStudentQuery("student-id", 1);
            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync((Exam?)null);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenExamEndTimeGreaterThanNow()
        {
            //Arrange
            var query = new GetExamReviewForCurrentStudentQuery("student-id", 1);
            var exam = new Exam
            {
                Id = query.ExamId,
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamResultDoesNotExist()
        {
            // Arrange
            var query = new GetExamReviewForCurrentStudentQuery("student-id", 1);
            var exam = new Exam
            {
                Id = query.ExamId,
                EndAt = DateTime.UtcNow.AddMinutes(-10)
            };

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);
            var examResults = new List<ExamResult>().BuildMock();

            _examResultRepoMock
                .Setup(x => x.GetAsQuery(true))
                .Returns(examResults);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenExamReviewExists()
        {
            // Arrange
            var query = new GetExamReviewForCurrentStudentQuery("student-id", 1);
            var exam = new Exam
            {
                Id = 1,
                EndAt = DateTime.UtcNow.AddMinutes(-10)
            };

            var examResult = new ExamResult("student-id", 1, 100, 80);

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);

            _examResultRepoMock
                .Setup(x => x.GetAsQuery(true))
                .Returns(new List<ExamResult> { examResult }.BuildMock());

            _studentAnswerRepoMock
                .Setup(x => x.GetAsQuery(true))
                .Returns(new List<StudentAnswer>().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }
    }
}
