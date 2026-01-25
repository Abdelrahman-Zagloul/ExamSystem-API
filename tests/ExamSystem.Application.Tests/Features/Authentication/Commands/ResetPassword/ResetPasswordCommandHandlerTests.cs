using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.Commands.ResetPassword;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System.Linq.Expressions;
using System.Text;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ResetPassword
{
    [Trait("Category", "Application.Authentication.ResetPassword.Handler")]
    public class ResetPasswordCommandHandlerTests
    {
        private readonly Mock<IAppEmailService> _appEmailServiceMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ResetPasswordCommandHandler _handler;

        public ResetPasswordCommandHandlerTests()
        {
            _appEmailServiceMock = new Mock<IAppEmailService>();
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();
            _handler = new ResetPasswordCommandHandler(_appEmailServiceMock.Object, _backgroundJobServiceMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundError_WhenUserNotExist()
        {
            //Arrange
            var command = new ResetPasswordCommand("email@gmail.com", "token", "newPassword");
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(command.Email))
                .ReturnsAsync((ApplicationUser)null!);

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnValidationError_WhenTokenOrNewPasswordIsInvalid()
        {
            //Arrange
            var user = new ApplicationUser { Email = "email@gmail.com" };
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("invalid-token"));
            var command = new ResetPasswordCommand("email@gmail.com", encodedToken, "newPassword");

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(user, "invalid-token", command.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError()));

            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenPasswordIsResetSuccessfully()
        {
            // Arrange
            var user = new ApplicationUser { Email = "email@gmail.com" };
            var token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("valid-token"));
            var command = new ResetPasswordCommand("email@gmail.com", token, "NewStrongPass123");

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(user, It.IsAny<string>(), command.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            _backgroundJobServiceMock
                .Setup(x => x.Enqueue(It.IsAny<Expression<Action>>()))
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _backgroundJobServiceMock.Verify(x => x.Enqueue(It.IsAny<Expression<Action>>()), Times.Once);
        }
    }
}
