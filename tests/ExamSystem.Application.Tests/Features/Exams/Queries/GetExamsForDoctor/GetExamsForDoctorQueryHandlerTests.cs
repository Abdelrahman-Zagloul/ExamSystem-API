using AutoMapper;
using ExamSystem.Application.Features.Exams.Queries.GetExamsForDoctor;
using ExamSystem.Application.Tests.Features.Exams.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Exams.Queries.GetExamsForDoctor
{
    [Trait("Category", "Application.Exam.GetExamsForDoctor.Handler")]
    public class GetExamsForDoctorQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly IMapper _mapper;

        private readonly GetExamsForDoctorQueryHandler _handler;

        public GetExamsForDoctorQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _mapper = MockHelper.CreateMappingProfile<TestExamMappingProfile>();
            _handler = new GetExamsForDoctorQueryHandler(_unitOfWorkMock.Object, _mapper);
        }

        private static GetExamsForDoctorQuery CreateQuery()
            => new GetExamsForDoctorQuery("doctor-id", null, 1, 20, "https://path", new Dictionary<string, string>());

        private static List<Exam> CreateExams()
        {
            var now = DateTime.UtcNow;
            return new List<Exam>
            {
                new Exam { Id = 1, DoctorId = "doctor-id",StartAt = now.AddMinutes(-10),EndAt = now.AddMinutes(10) },           // Active exam
                new Exam { Id = 2, DoctorId = "doctor-id", StartAt = now.AddHours(1),EndAt = now.AddHours(2) },                 // Upcoming exam
                new Exam { Id = 3, DoctorId = "another-doctor",StartAt = now.AddMinutes(-10),EndAt = now.AddMinutes(10) },      // Finished exam
            };
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyExamsForDoctor()
        {
            // Arrange
            var query = CreateQuery();

            _examRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(CreateExams().BuildMock());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ShouldApplyExamStatusFilter_WhenStatusProvided()
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
