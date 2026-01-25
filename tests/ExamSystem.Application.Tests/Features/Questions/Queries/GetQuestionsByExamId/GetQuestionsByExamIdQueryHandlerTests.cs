using AutoMapper;
using ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId;
using ExamSystem.Application.Mappers;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Questions.Queries.GetQuestionsByExamId
{
    [Trait("Category", "Application.Question.GetQuestionByExamId.Handler")]
    public class GetQuestionsByExamIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly GetQuestionsByExamIdQueryHandler _handler;
        private readonly Mock<IGenericRepository<Question>> _questionRepoMock;
        public GetQuestionsByExamIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<QuestionMappingProfile>();
            _questionRepoMock = new Mock<IGenericRepository<Question>>();
            _unitOfWorkMock.Setup(u => u.Repository<Question>()).Returns(_questionRepoMock.Object);
            _handler = new GetQuestionsByExamIdQueryHandler(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ShouldReturnPaginatedResult_WhenQuestionsExist()
        {
            // Arrange
            var query = new GetQuestionsByExamIdQuery("doctor-id", 1, 1, 2, "https://example.com", new Dictionary<string, string>());

            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    ExamId = 1,
                    QuestionType = QuestionType.TrueFalse,
                    Exam = new Exam { DoctorId = "doctor-id" },
                    Options = new List<Option>()
                },
                new Question
                {
                    Id = 2,
                    ExamId = 1,
                    QuestionType = QuestionType.TrueFalse,
                    Exam = new Exam { DoctorId = "doctor-id" },
                    Options = new List<Option>()
                },
                new Question
                {
                    Id = 3,
                    ExamId = 1,
                    QuestionType = QuestionType.MCQ,
                    Exam = new Exam { DoctorId = "doctor-id" },
                    Options = new List<Option>()
                }
            }.BuildMock();

            _questionRepoMock.Setup(x => x.GetAsQuery(true))
                .Returns(questions);

            _questionRepoMock
                .Setup(x => x.CountAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Question, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Items.Count.Should().Be(2);
            result.Value.PageNumber.Should().Be(1);
            result.Value.PageSize.Should().Be(2);
        }
    }
}


