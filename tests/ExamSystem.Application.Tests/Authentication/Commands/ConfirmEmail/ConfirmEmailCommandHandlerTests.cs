using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System.Linq.Expressions;
using System.Text;

namespace ExamSystem.Application.Tests.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IAppEmailService> _appEmailServiceMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
        private readonly ConfirmEmailCommandHandler _handler;

        public ConfirmEmailCommandHandlerTests()
        {
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _appEmailServiceMock = new Mock<IAppEmailService>();
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();

            _handler = new ConfirmEmailCommandHandler(_userManagerMock.Object, _jwtTokenServiceMock.Object, _appEmailServiceMock.Object, _backgroundJobServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenTokenIsValid()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@test.com", FullName = "Test User" };
            var rawToken = "valid-confirmation-token";
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            var command = new ConfirmEmailCommand(user.Email, encodedToken);
            var expectedAuthDto = new AuthDto("jwt-token", "Role", user.Id, DateTime.UtcNow.AddMinutes(30));

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, rawToken)).ReturnsAsync(IdentityResult.Success);
            _jwtTokenServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync(expectedAuthDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedAuthDto);

            _backgroundJobServiceMock.Verify(x => x.Enqueue(It.IsAny<Expression<Action>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var command = new ConfirmEmailCommand("nonexistent@test.com", "any-token");
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnValidationFail_WhenIdentityResultFails()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@test.com" };
            var rawToken = "invalid-token";
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            var command = new ConfirmEmailCommand(user.Email, encodedToken);

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ConfirmEmailAsync(user, rawToken))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}
