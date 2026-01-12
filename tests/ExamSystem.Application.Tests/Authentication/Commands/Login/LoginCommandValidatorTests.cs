using ExamSystem.Application.Features.Authentication.Commands.Login;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Authentication.Commands.Login
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator = new LoginCommandValidator();

        [Fact]
        public void Should_HaveError_When_EmailIsEmpty()
        {
            // Arrange
            var command = new LoginCommand("", "12345678");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Email" && x.ErrorMessage == "Email is required.");
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            // Arrange
            var command = new LoginCommand("invalid-email", "12345678");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Email" && x.ErrorMessage == "Invalid email format.");
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsEmpty()
        {
            // Arrange
            var command = new LoginCommand("test@test.com", "");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Password" && x.ErrorMessage == "Password is required.");
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsTooShort()
        {
            // Arrange
            var command = new LoginCommand("test@test.com", "123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Password" && x.ErrorMessage == "Password must be at least 6 characters.");
        }

        [Fact]
        public void Should_Pass_When_CommandIsValid()
        {
            // Arrange
            var command = new LoginCommand("test@test.com", "12345678");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
