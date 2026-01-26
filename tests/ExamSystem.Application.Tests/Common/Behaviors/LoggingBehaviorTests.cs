using ExamSystem.Application.Common.Behaviors;
using ExamSystem.Application.Contracts.Identity;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExamSystem.Application.Tests.Common.Behaviors
{
    [Trait("Category", "Application.Behaviors.LoggingBehavior")]
    public class LoggingBehaviorTests
    {
        public record TestCommand() : IRequest<string>;

        [Fact]
        public async Task Handle_ShouldCallNext_AndLogInformation_WhenRequestSucceeds()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoggingBehavior<TestCommand, string>>>();
            var currentUserMock = new Mock<ICurrentUserService>();

            currentUserMock.Setup(x => x.UserId).Returns("user-123");
            var behavior = new LoggingBehavior<TestCommand, string>(loggerMock.Object, currentUserMock.Object);
            var nextCalled = false;

            RequestHandlerDelegate<string> next = async _ =>
            {
                nextCalled = true;
                return "OK";
            };

            var command = new TestCommand();


            // Act
            var result = await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            nextCalled.Should().BeTrue();
            result.Should().Be("OK");
        }

        [Fact]
        public async Task Handle_ShouldLogError_AndRethrowException_WhenHandlerThrows()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoggingBehavior<TestCommand, string>>>();
            var currentUserMock = new Mock<ICurrentUserService>();

            currentUserMock.Setup(x => x.UserId).Returns("User-123");

            var behavior = new LoggingBehavior<TestCommand, string>(loggerMock.Object, currentUserMock.Object);

            RequestHandlerDelegate<string> next = async _ => throw new InvalidOperationException();
            var command = new TestCommand();

            // Act
            Func<Task> act = async () => await behavior.Handle(command, next, CancellationToken.None);


            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
