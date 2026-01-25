using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.Queries.GetQuestionById;
using ExamSystem.Application.Mappers;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Questions.Queries.GetQuestionById
{
    [Trait("Category", "Application.Question.GetQuestionById.Handler")]
    public class GetQuestionByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly IMapper _mapper;
        private readonly GetQuestionByIdQueryHandler _handler;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<Question>> _questionRepoMock;
        public GetQuestionByIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapper = MockHelper.CreateMappingProfile<QuestionMappingProfile>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _questionRepoMock = new Mock<IGenericRepository<Question>>();
            _handler = new GetQuestionByIdQueryHandler(_unitOfWorkMock.Object, _mapper);
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<Question>()).Returns(_questionRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            //Arrange
            var query = new GetQuestionByIdQuery("user-Id", 1, 1);

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
        public async Task Handle_ShouldReturnForbidden_WhenDoctorIdNotEqualExamDoctorId()
        {
            //Arrange
            var query = new GetQuestionByIdQuery("user-Id", 1, 1);
            var exam = new Exam { DoctorId = "different-user-Id" };
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
        public async Task Handle_ShouldReturnNotFound_WhenQuestionResponseIsNull()
        {
            //Arrange
            var query = new GetQuestionByIdQuery("user-Id", 1, 1);
            var exam = new Exam { DoctorId = "user-Id" };
            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);

            var emptyQuestions = new List<Question>().BuildMock();
            _questionRepoMock
                .Setup(x => x.GetAsQuery(true))
                .Returns(emptyQuestions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.NotFound);

        }

        [Fact]
        public async Task Handle_ShouldReturnQuestion_WhenExists()
        {
            //Arrange
            var query = new GetQuestionByIdQuery("user-Id", 1, 1);

            var exam = new Exam { DoctorId = "user-Id" };
            _examRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), query.ExamId))
                .ReturnsAsync(exam);

            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    QuestionMark =1,
                    Options = new List<Option>(),
                    StudentAnswers = new List<StudentAnswer>(),
                    Exam = new Exam(),
                    CorrectOption = null
                }
            }.BuildMock();

            _questionRepoMock
                .Setup(x => x.GetAsQuery(true))
                .Returns(questions);

            //Act
            var result = await _handler.Handle(query, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.QuestionMark.Should().Be(1);
        }
    }
}




