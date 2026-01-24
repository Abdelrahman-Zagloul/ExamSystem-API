using ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ResendConfirmEmail
{

    [Trait("Layer", "Application")]
    [Trait("Feature", "Authentication")]
    [Trait("Action", "ResendConfirmEmail")]
    [Trait("Component", "Validator")]
    public class ResendConfirmEmailCommandValidatorTests
    {
        private readonly ResendConfirmEmailCommandValidator _validator = new ResendConfirmEmailCommandValidator();

        [Theory]
        [InlineData(null)]                                              // email is null
        [InlineData("")]                                                // email is empty
        [InlineData("invalid-email-format")]                            // invalid email format 
        public void Validate_ShouldHaveError_WhenInvalidInputs(string email)
        {
            // Arrange
            var command = new ResendConfirmEmailCommand(email);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ShouldPass_WhenInputIsValid()
        {
            // Arrange
            var command = new ResendConfirmEmailCommand("test@gmail.com");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
