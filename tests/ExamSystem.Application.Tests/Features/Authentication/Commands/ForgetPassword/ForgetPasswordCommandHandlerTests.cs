using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.Commands.ForgetPassword;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ForgetPassword
{
    [Trait("Category", "Application.Authentication.ForgetPassword.Handler")]
    public class ForgetPasswordCommandHandlerTests
    {
        private readonly Mock<IAppEmailService> _emailServiceMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ForgetPasswordCommandHandler _handler;

        public ForgetPasswordCommandHandlerTests()
        {
            _emailServiceMock = new Mock<IAppEmailService>();
            _backgroundJobMock = new Mock<IBackgroundJobService>();
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();

            _handler = new ForgetPasswordCommandHandler(_userManagerMock.Object, _emailServiceMock.Object, _backgroundJobMock.Object);
        }
        [Fact]
        public async Task Handle_ShouldSendResetPasswordEmail_WhenUserExists()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" };

            _userManagerMock
                .Setup(x => x.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _userManagerMock
                .Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token");

            _backgroundJobMock
                .Setup(x => x.Enqueue(It.IsAny<Expression<Action>>()))
                .Verifiable();

            var command = new ForgetPasswordCommand(user.Email);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _backgroundJobMock.Verify(x => x.Enqueue(It.IsAny<Expression<Action>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotSendEmailButReturnSuccess_WhenUserNotExist()
        {
            // Arrange
            _userManagerMock
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null!);

            var command = new ForgetPasswordCommand("email@example.com");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _backgroundJobMock.Verify(x => x.Enqueue(It.IsAny<Expression<Action>>()), Times.Never);
        }
    }
}










