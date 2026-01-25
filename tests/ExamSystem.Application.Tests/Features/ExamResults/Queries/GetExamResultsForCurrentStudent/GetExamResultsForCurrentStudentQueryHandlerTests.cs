using AutoMapper;
using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForCurrentStudent;
using ExamSystem.Application.Tests.Features.ExamResults.Mapping;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.ExamResults.Queries.GetExamResultsForCurrentStudent
{
    [Trait("Category", "Application.Question.ExamResultsForCurrentStudent.Handler")]
    public class GetExamResultsForCurrentStudentQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly GetExamResultsForCurrentStudentQueryHandler _handler;
        private readonly Mock<IGenericRepository<ExamResult>> _examResultRepositoryMock;
        public GetExamResultsForCurrentStudentQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<TestExamResultMappingProfile>();
            _examResultRepositoryMock = new Mock<IGenericRepository<ExamResult>>();
            _unitOfWorkMock.Setup(x => x.Repository<ExamResult>()).Returns(_examResultRepositoryMock.Object);
            _handler = new GetExamResultsForCurrentStudentQueryHandler(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedExamResults_ForCurrentStudent()
        {
            // Arrange
            var query = new GetExamResultsForCurrentStudentQuery("studentId", 1, 10, "https://api/exams/results", new Dictionary<string, string>());

            var examResultsList = new List<ExamResult>
            {
                new ExamResult("studentId", 1, 100, 80),
                new ExamResult("studentId", 2, 100, 90)
            }.BuildMock();

            foreach (var examResult in examResultsList)
                PrivatePropertySetter.Set(examResult, nameof(Exam), new Exam
                {
                    Id = examResult.ExamId,
                    EndAt = DateTime.UtcNow.AddMinutes(-10)
                });

            _examResultRepositoryMock.Setup(x => x.GetAsQuery(true)).Returns(examResultsList);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            result.IsSuccess.Should().BeTrue();
            result.Value.Items.Count.Should().Be(2);
        }

    }
}
