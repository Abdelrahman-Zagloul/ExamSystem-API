using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace ExamSystem.Application.Tests.Features.Questions.Commands.DeleteQuestion
{
    [Trait("Category", "Application.Question.Delete.Handler")]
    public class DeleteQuestionCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeleteQuestionCommandHandler _handler;
        private readonly Mock<IGenericRepository<Question>> _questionRepoMock;
        public DeleteQuestionCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteQuestionCommandHandler(_unitOfWorkMock.Object);
            _questionRepoMock = new Mock<IGenericRepository<Question>>();
            _unitOfWorkMock.Setup(x => x.Repository<Question>()).Returns(_questionRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenQuestionDoesNotExist()
        {
            //Arrange
            var command = new DeleteQuestionCommand(1, 1);

            _questionRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.QuestionId))
                .ReturnsAsync((Question?)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamIdNotEqualExamIdForQuestion()
        {
            //Arrange
            var command = new DeleteQuestionCommand(1, 1);

            var question = new Question() { Id = 1, ExamId = 2 };
            _questionRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.QuestionId))
                .ReturnsAsync(question);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldDeleteQuestion_WhenQuestionExistsAndExamIdMatches()
        {
            // Arrange
            var command = new DeleteQuestionCommand(1, 1);

            var question = new Question { Id = 1, ExamId = 1 };

            _questionRepoMock
                .Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.QuestionId))
                .ReturnsAsync(question);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _questionRepoMock.Verify(x => x.Remove(question), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
