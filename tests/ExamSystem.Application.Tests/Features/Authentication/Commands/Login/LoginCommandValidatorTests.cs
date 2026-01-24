using ExamSystem.Application.Features.Authentication.Commands.Login;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.Login
{
    [Trait("Layer", "Application")]
    [Trait("Feature", "Authentication")]
    [Trait("Action", "Login")]
    [Trait("Component", "Validator")]
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator = new LoginCommandValidator();

        [Theory]
        [InlineData("", "12345678")]                            // Empty email
        [InlineData("invalid-email", "12345678")]               // Invalid email
        [InlineData("test@test.com", "")]                       // Empty password
        [InlineData("test@test.com", "123")]                    // Too short password
        public void Validate_ShouldHaveError_ForInvalidInputs(string email, string password)
        {
            // Arrange
            var command = new LoginCommand(email, password, "IP_Address");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ShouldPass_When_CommandIsValid()
        {
            // Arrange
            var command = new LoginCommand("test@test.com", "12345678", "IP_Address");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
