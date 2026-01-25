using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor;
using ExamSystem.Application.Tests.Features.ExamResults.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.ExamResults.Queries.GetExamResultsForDoctor
{

    [Trait("Category", "Application.Question.ExamResultsForDoctor.Handler")]
    public class GetExamResultsForDoctorQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<ExamResult>> _examResultRepoMock;
        private readonly GetExamResultsForDoctorQueryHandler _handler;
        private static GetExamResultsForDoctorQuery CreateValidQuery() =>
            new GetExamResultsForDoctorQuery("doctor-id", 1, null, 1, 10, "http://baseurl", new Dictionary<string, string>());

        public GetExamResultsForDoctorQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<TestExamResultMappingProfile>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _examResultRepoMock = new Mock<IGenericRepository<ExamResult>>();
            _handler = new GetExamResultsForDoctorQueryHandler(_unitOfWorkMock.Object, _mapper);
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<ExamResult>()).Returns(_examResultRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            //Arrange
            var query = CreateValidQuery();
            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync((Exam?)null);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnForbidden_WhenDoctorIdOfExamNotEqualDoctorIdFromRequest()
        {
            //Arrange
            var query = CreateValidQuery();
            var exam = new Exam { DoctorId = "different-doctor-id" };

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Forbidden);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenEndTimeOfExamIsGreaterThanNow()
        {
            //Arrange
            var query = CreateValidQuery();
            var exam = new Exam { DoctorId = "doctor-id", EndAt = DateTime.UtcNow.AddDays(1) };

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Conflict);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResult_WhenQueryIsValid()
        {
            //Arrange
            var query = CreateValidQuery();
            var exam = new Exam { DoctorId = "doctor-id", EndAt = DateTime.UtcNow.AddDays(-1) };
            var examResults = new List<ExamResult>()
            {
                new ExamResult( "student_id_1",query.ExamId, 100, 85),
                new ExamResult( "student_id_2",query.ExamId, 100, 70),
                new ExamResult( "student_id_3",query.ExamId, 100, 60),
            }.BuildMock();

            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                    .ReturnsAsync(exam);

            _examResultRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(examResults);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(3);
        }
    }
}
