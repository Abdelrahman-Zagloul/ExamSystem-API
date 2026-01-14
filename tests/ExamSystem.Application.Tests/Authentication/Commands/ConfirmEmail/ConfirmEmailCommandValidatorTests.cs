using ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Authentication.Commands.ConfirmEmail
{
    public class ConfirmEmailCommandValidatorTests
    {
        private readonly ConfirmEmailCommandValidator _validator = new ConfirmEmailCommandValidator();

        [Theory]
        [InlineData("", "")]                                            // Both fields empty
        [InlineData("", "valid-token")]                                 // Missing email only
        [InlineData("invalid-email", "valid-token")]                    // Bad email format
        [InlineData("valid-email", "")]                                 // Missing token only
        public void Validate_ShouldHaveError_WhenInvalidInputs(string email, string token)
        {
            // Arrange
            var command = new ConfirmEmailCommand(email, token);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ShouldPass_WhenInputIsValid()
        {
            // Arrange
            var command = new ConfirmEmailCommand("test@gmail.com", "token");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
