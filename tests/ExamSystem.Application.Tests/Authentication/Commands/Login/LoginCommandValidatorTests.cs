using ExamSystem.Application.Features.Authentication.Commands.Login;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Authentication.Commands.Login
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator = new LoginCommandValidator();

        [Theory]
        [InlineData("", "12345678")]                            // Empty email
        [InlineData("invalid-email", "12345678")]               // Invalid email
        [InlineData("test@test.com", "")]                       // Empty password
        [InlineData("test@test.com", "123")]                    // Too short password
        public void LoginCommandValidator_ShouldHaveError_WhenInvalidInputs(string email, string password)
        {
            // Arrange
            var command = new LoginCommand(email, password);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void LoginCommandValidator_ShouldPass_WhenInputIsValid()
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
