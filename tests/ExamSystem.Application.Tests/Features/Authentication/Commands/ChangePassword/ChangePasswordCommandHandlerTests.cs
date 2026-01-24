using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.Commands.ChangePassword;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ChangePassword
{
    [Trait("Layer", "Application")]
    [Trait("Feature", "Authentication")]
    [Trait("Action", "ChangePassword")]
    [Trait("Component", "Handler")]
    public class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<IAppEmailService> _appEmailServiceMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();
            _appEmailServiceMock = new Mock<IAppEmailService>();
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();
            _handler = new ChangePasswordCommandHandler(_appEmailServiceMock.Object, _backgroundJobServiceMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var command = new ChangePasswordCommand("invalid-id", "OldPass123!", "NewPass123!");

            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenCurrentPasswordIsIncorrect()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-1", Email = "test@test.com" };
            var command = new ChangePasswordCommand(user.Id, "WrongOldPass", "NewPass123!");

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id)).ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenPasswordChangeFails()
        {
            // Arrange
            var user = new ApplicationUser { Id = "user-1", Email = "test@test.com" };
            var command = new ChangePasswordCommand(user.Id, "OldPass123!", "NewPass123!");

            _userManagerMock.Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _backgroundJobServiceMock.Verify(x => x.Enqueue(It.IsAny<Expression<Action>>()), Times.Once);
        }
    }
}