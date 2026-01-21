using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Features.Authentication.Commands.Login;
using ExamSystem.Application.Features.Authentication.DTOs;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace ExamSystem.Application.Tests.Authentication.Commands.Login
{
    public class LoginCommandHandlerTests
    {
        private readonly LoginCommandHandler _handler;
        private readonly Mock<IAccessTokenService> _jwtTokenServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public LoginCommandHandlerTests()
        {
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();
            _jwtTokenServiceMock = new Mock<IAccessTokenService>();
            _handler = new LoginCommandHandler(_userManagerMock.Object, _jwtTokenServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_When_CredentialsAreValid()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "test@test.com",
                EmailConfirmed = true
            };
            var command = new LoginCommand(user.Email, "123456");
            var authDto = new AuthDto("fake-jwt-token", "fake-role", "fake-userId", DateTime.UtcNow.AddMinutes(30));

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
            _jwtTokenServiceMock.Setup(x => x.GenerateTokenAsync(user)).ReturnsAsync(authDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(authDto);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_When_CredentialsAreInvalid()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "test@test.com",
                EmailConfirmed = true
            };
            var command = new LoginCommand(user.Email, "wrong-password");

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, command.Password)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_When_EmailIsNotConfirmed()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Email = "test@test.com",
                EmailConfirmed = false
            };
            var command = new LoginCommand(user.Email, "123456");

            _userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
            _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }
    }
}
