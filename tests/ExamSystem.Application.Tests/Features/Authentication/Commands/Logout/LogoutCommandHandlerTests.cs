using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.Commands.Logout;
using FluentAssertions;
using Moq;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.Logout
{
    [Trait("Category", "Application.Authentication.Logout.Handler")]
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly LogoutCommandHandler _handler;

        public LogoutCommandHandlerTests()
        {
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _handler = new LogoutCommandHandler(_refreshTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldRevokeAllRefreshTokensAndReturnSuccessResult_WhenUserIsLoggedIn()
        {
            // Arrange
            var command = new LogoutCommand("user-id", "127.0.0.1");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Logged out successfully");

            _refreshTokenServiceMock.Verify(
                x => x.RevokeAllAsync(
                    "user-id",
                    "127.0.0.1",
                    It.IsAny<CancellationToken>()),
                Times.Once
            );
        }
    }
}