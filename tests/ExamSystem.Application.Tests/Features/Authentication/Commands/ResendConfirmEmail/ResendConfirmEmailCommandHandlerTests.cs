using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ResendConfirmEmail
{
    [Trait("Category", "Application.Authentication.ResendConfirmEmail.Handler")]
    public class ResendConfirmEmailCommandHandlerTests
    {
        private readonly Mock<IAppEmailService> _emailServiceMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly ResendConfirmEmailCommandHandler _handler;

        public ResendConfirmEmailCommandHandlerTests()
        {
            _emailServiceMock = new Mock<IAppEmailService>();
            _backgroundJobMock = new Mock<IBackgroundJobService>();
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();

            _handler = new ResendConfirmEmailCommandHandler(_emailServiceMock.Object, _backgroundJobMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnsOkWithoutSendAnyThing_WhenUserNotFound()
        {
            // Arrange
            var command = new ResendConfirmEmailCommand("test@test.com");
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync((ApplicationUser)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldReturnsConflict_WhenEmailAlreadyConfirmed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "confirmed@test.com",
                EmailConfirmed = true
            };

            var command = new ResendConfirmEmailCommand(user.Email);
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldEnqueuesEmailAndReturnsOk_WhenUserNotConfirmed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "ali@test.com",
                EmailConfirmed = false
            };

            var command = new ResendConfirmEmailCommand(user.Email);
            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);

            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("fake-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();

            _backgroundJobMock.Verify(x => x.Enqueue(It.IsAny<Expression<Action>>()), Times.Once);
        }
    }
}

