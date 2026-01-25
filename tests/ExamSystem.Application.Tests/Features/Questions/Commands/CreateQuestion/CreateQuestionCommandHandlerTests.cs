using AutoMapper;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Questions.Commands.CreateQuestion
{

    [Trait("Category", "Application.Question.Create.Handler")]
    public class CreateQuestionCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateQuestionCommandHandler _handler;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<Question>> _questionRepoMock;

        public CreateQuestionCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new CreateQuestionCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object);
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _questionRepoMock = new Mock<IGenericRepository<Question>>();

            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<Question>()).Returns(_questionRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            //Arrange
            var command = new CreateQuestionCommand(1, "Question", 5, QuestionType.TrueFalse, new List<CreateOptionRequest>(), 1);

            _examRepoMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Exam, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.First().ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenExamExists()
        {
            // Arrange
            var command = new CreateQuestionCommand(1, "Question", 5, QuestionType.TrueFalse, new List<CreateOptionRequest>(), 1);
            var question = new Question()
            {
                Options = new List<Option>
                {
                    new Option { Id = 10 },
                    new Option { Id = 20 }
                }
            };

            _examRepoMock
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Exam, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(m => m.Map<Question>(command))
                .Returns(question);

            _questionRepoMock
                .Setup(r => r.AddAsync(question, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldSetCorrectOptionId_WhenResultIsSuccess()
        {
            // Arrange
            var command = new CreateQuestionCommand(1, "Question", 5, QuestionType.TrueFalse, new List<CreateOptionRequest>(), 2);
            var question = new Question
            {
                Options = new List<Option>
                {
                    new Option { Id = 100 },
                    new Option { Id = 200 }
                }
            };

            _examRepoMock
                .Setup(x => x.AnyAsync(It.IsAny<Expression<Func<Exam, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(m => m.Map<Question>(command))
                .Returns(question);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            question.CorrectOptionId.Should().Be(200);
        }

        [Fact]
        public async Task Handle_ShouldCreateQuestion_WhenExamExistsAndSetCorrectOptionIdCorrectly()
        {
            // Arrange
            var command = new CreateQuestionCommand(1, "Question", 5,
                QuestionType.TrueFalse, new List<CreateOptionRequest>(), 1);

            var question = new Question
            {
                Options = new List<Option>
                {
                    new Option { Id = 10 },
                    new Option { Id = 20 }
                }
            };

            _examRepoMock
                .Setup(r => r.AnyAsync(It.IsAny<Expression<Func<Exam, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(m => m.Map<Question>(command))
                .Returns(question);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _questionRepoMock.Verify(r => r.AddAsync(question, It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

    }
}
