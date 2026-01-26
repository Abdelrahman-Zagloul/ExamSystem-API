using AutoMapper;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent;
using ExamSystem.Application.Tests.Features.Exams.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Exams.Queries.GetExamsForStudent
{
    [Trait("Category", "Application.Exam.GetExamsForStudent.Handler")]
    public class GetExamsForStudentQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly IMapper _mapper;
        private readonly GetExamsForStudentQueryHandler _handler;

        public GetExamsForStudentQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _mapper = MockHelper.CreateMappingProfile<TestExamMappingProfile>();
            _handler = new GetExamsForStudentQueryHandler(_unitOfWorkMock.Object, _mapper);
        }

        private static GetExamsForStudentQuery CreateQuery() =>
             new GetExamsForStudentQuery(null, 1, 20, "https://path", new Dictionary<string, string>());

        private static List<Exam> CreateExams()
        {
            var now = DateTime.UtcNow;
            return new List<Exam>
            {

                new Exam{ Id = 1,StartAt = now.AddMinutes(-10),EndAt = now.AddMinutes(10)},         // Active exam
                new Exam{ Id = 2,StartAt = now.AddHours(1),EndAt = now.AddHours(2)},                // Upcoming exam
                new Exam{ Id = 3,StartAt = now.AddHours(-2),EndAt = now.AddHours(-1)},              // Finished exam
            };
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyActiveExams_WhenStatusIsActive()
        {
            // Arrange
            var query = CreateQuery() with { ExamStatus = ExamStatus.Active };

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(CreateExams().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyUpcomingExams_WhenStatusIsUpcoming()
        {
            // Arrange
            var query = CreateQuery() with { ExamStatus = ExamStatus.Upcoming };

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(CreateExams().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyFinishedExams_WhenStatusIsFinished()
        {
            // Arrange
            var query = CreateQuery() with { ExamStatus = ExamStatus.Finished };

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(CreateExams().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResult()
        {
            // Arrange
            var query = CreateQuery() with { PageNumber = 1, PageSize = 1 };

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(CreateExams().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(1);
            result.Value.PageNumber.Should().Be(1);
            result.Value.PageSize.Should().Be(1);
        }
    }
}
