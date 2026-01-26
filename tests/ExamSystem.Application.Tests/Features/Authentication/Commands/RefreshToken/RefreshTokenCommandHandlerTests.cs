using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.Commands.RefreshToken;
using ExamSystem.Application.Features.Authentication.Shared;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.RefreshToken
{
    [Trait("Category", "Application.Authentication.RefreshToken.Handler")]
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
        private readonly Mock<IAccessTokenService> _accessTokenServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
            _accessTokenServiceMock = new Mock<IAccessTokenService>();
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();
            _handler = new RefreshTokenCommandHandler(
                _refreshTokenServiceMock.Object,
                _accessTokenServiceMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenRefreshTokenIsNull()
        {
            // Arrange
            var command = new RefreshTokenCommand(null, "ip-address");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.Unauthorized);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenRotateRefreshTokenFails()
        {
            // Arrange
            var serviceError = Error.Unauthorized("InvalidToken", "Refresh token is invalid");

            _refreshTokenServiceMock
                .Setup(x => x.RotateAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenDto>.Fail(serviceError));

            var command = new RefreshTokenCommand("invalid-token", "ip-address");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e == serviceError);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var refreshDto = new RefreshTokenDto("new-refresh-token", DateTime.UtcNow.AddMinutes(30), "user-id");

            _refreshTokenServiceMock
                .Setup(x => x.RotateAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenDto>.Ok(refreshDto));

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user-id"))
                .ReturnsAsync((ApplicationUser?)null);

            var command = new RefreshTokenCommand("valid-token", "ip-address");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorType == ErrorType.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldReturnAccessAndRefreshTokens_WhenEverythingIsValid()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-id" };
            var refreshDto = new RefreshTokenDto("new-refresh-token", DateTime.UtcNow.AddMinutes(30), user.Id);

            var accessDto = new AccessTokenResponse("access-token", "role", user.Id, DateTime.UtcNow.AddMinutes(30));

            _refreshTokenServiceMock
                .Setup(x => x.RotateAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<RefreshTokenDto>.Ok(refreshDto));

            _userManagerMock
                .Setup(x => x.FindByIdAsync("user-id"))
                .ReturnsAsync(user);

            _accessTokenServiceMock
                .Setup(x => x.GenerateTokenAsync(user))
                .ReturnsAsync(accessDto);

            var command = new RefreshTokenCommand("valid-token", "ip-address");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            _refreshTokenServiceMock.Verify(
                x => x.RotateAsync(
                    "valid-token",
                    "ip-address",
                    It.IsAny<CancellationToken>()),
                Times.Once
            );

            _accessTokenServiceMock.Verify(
                x => x.GenerateTokenAsync(user),
                Times.Once
            );
        }
    }
}