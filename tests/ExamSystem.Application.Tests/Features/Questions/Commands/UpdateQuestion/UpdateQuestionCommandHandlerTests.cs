using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Interfaces;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Questions.Commands.UpdateQuestion
{
    [Trait("Category", "Application.Question.Update.Handler")]
    public class UpdateQuestionCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateQuestionCommandHandler _handler;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<Question>> _questionRepoMock;

        public UpdateQuestionCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateQuestionCommandHandler(_unitOfWorkMock.Object);

            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _questionRepoMock = new Mock<IGenericRepository<Question>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<Question>()).Returns(_questionRepoMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenExamDoesNotExist()
        {
            //Arrange
            var command = new UpdateQuestionCommand(1, 1, null, null, null, null);

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync((Exam?)null);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }
        [Fact]
        public async Task Handle_ShouldReturnBadRequest_WhenExamAlreadyStarted()
        {
            //Arrange
            var command = new UpdateQuestionCommand(1, 1, null, null, null, null);
            var exam = new Exam { Id = 1, StartAt = DateTime.UtcNow.AddMinutes(-1) };

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.BadRequest);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenQuestionDoesNotExist()
        {
            //Arrange
            var command = new UpdateQuestionCommand(1, 1, "new", 5, null, null);

            var exam = new Exam { Id = 1, StartAt = DateTime.UtcNow.AddMinutes(10) };

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), command.ExamId))
                .ReturnsAsync(exam);

            _questionRepoMock.Setup(x => x.GetAsQuery(false))
                .Returns(new List<Question>().BuildMock());

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenOptionDoesNotExist()
        {
            var command = new UpdateQuestionCommand(1, 1, null, null, [new UpdateOptionRequest(99, "new text")], null);
            var exam = new Exam { Id = 1, StartAt = DateTime.UtcNow.AddMinutes(10) };
            var question = new Question
            {
                Id = 1,
                ExamId = 1,
                Options = new List<Option>
                {
                    new Option { Id = 1 },
                    new Option { Id = 2 },
                }
            };

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), 1))
                    .ReturnsAsync(exam);

            _questionRepoMock.Setup(x => x.GetAsQuery(false))
                    .Returns(new List<Question> { question }.BuildMock());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldUpdateQuestion_WhenDataIsValid()
        {
            var command = new UpdateQuestionCommand(1, 1, null, null, [new UpdateOptionRequest(1, "new text")], 1);
            var exam = new Exam { Id = 1, StartAt = DateTime.UtcNow.AddMinutes(10) };
            var option = new Option { Id = 1, OptionText = "Old" };
            var question = new Question
            {
                Id = 1,
                ExamId = 1,
                QuestionText = "Old",
                QuestionMark = 5,
                Options = new List<Option> { option }
            };

            _examRepoMock.Setup(x => x.FindAsync(It.IsAny<CancellationToken>(), 1))
                .ReturnsAsync(exam);

            _questionRepoMock.Setup(x => x.GetAsQuery(false))
                .Returns(new List<Question> { question }.BuildMock());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            question.QuestionMark.Should().Be(5);
            question.CorrectOptionId.Should().Be(1);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}