using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.Commands.RevokeToken;
using FluentAssertions;
using Moq;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.RevokeToken
{
    [Trait("Category", "Application.Authentication.RevokeToken.Handler")]
    public class RevokeTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly RevokeTokenCommandHandler _handler;

        public RevokeTokenCommandHandlerTests()
        {
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _handler = new RevokeTokenCommandHandler(_refreshTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorizedError_WhenRefreshTokenIsNull()
        {
            // Arrange
            var command = new RevokeTokenCommand(null, "ip-address");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Unauthorized);
        }

        [Fact]
        public async Task Handle_ShouldCallRevokeAsync_WhenRefreshTokenIsValid()
        {
            // Arrange
            _refreshTokenServiceMock
                .Setup(x => x.RevokeAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok());

            var command = new RevokeTokenCommand("valid-token", "ip-address");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _refreshTokenServiceMock.Verify(x => x.RevokeAsync("valid-token", "ip-address", It.IsAny<CancellationToken>()), Times.Once
            );
        }
    }
}