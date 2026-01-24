using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Features.Authentication.Commands.Register;
using ExamSystem.Application.Features.Authentication.Shared;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.Register
{

    [Trait("Layer", "Application")]
    [Trait("Feature", "Authentication")]
    [Trait("Action", "Register")]
    [Trait("Component", "Handler")]
    public class RegisterCommandHandlerTests
    {
        private readonly Mock<IAppEmailService> _appEmailServiceMock;
        private readonly Mock<IBackgroundJobService> _backgroundJobServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly RegisterCommandHandler _handler;
        public RegisterCommandHandlerTests()
        {
            _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();
            _appEmailServiceMock = new Mock<IAppEmailService>();
            _backgroundJobServiceMock = new Mock<IBackgroundJobService>();
            _handler = new RegisterCommandHandler(_appEmailServiceMock.Object, _backgroundJobServiceMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnConflict_WhenEmailAlreadyExists()
        {
            //Arrange
            var command = new RegisterCommand("test", "test@example.com", "Password123", "Password123", RoleDto.Student);
            var usersMock = (new List<ApplicationUser> { new ApplicationUser { Email = "test@example.com" } }).BuildMock();

            _userManagerMock.Setup(x => x.Users).Returns(usersMock);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);

            //Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnValidationError_WhenUserCreationFails()
        {
            // Arrange
            var command = new RegisterCommand("Test", "test@example.com", "Pass", "Pass", RoleDto.Student);

            _userManagerMock.Setup(x => x.Users).Returns(new List<ApplicationUser>().BuildMock());
            var identityError = IdentityResult.Failed(new IdentityError { Description = "Password too weak" });

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(identityError);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnValidationError_WhenRoleAssignmentFails()
        {
            // Arrange
            var command = new RegisterCommand("Test", "test@example.com", "Password123", "Password123", RoleDto.Doctor);
            _userManagerMock.Setup(x => x.Users).Returns(new List<ApplicationUser>().BuildMock());

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var roleError = IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(roleError);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenRegistrationIsSuccessful()
        {
            var command = new RegisterCommand("Full Name", "test@example.com", "Password123!", "Password123!", RoleDto.Student);

            // Mock empty user list
            var usersMock = new List<ApplicationUser>().BuildMock();
            _userManagerMock.Setup(x => x.Users).Returns(usersMock);

            // Mock Success Results
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("fake-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
