using ExamSystem.Application.Features.Authentication.Commands.ForgetPassword;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ForgetPassword
{
    [Trait("Category", "Application.Authentication.ForgetPassword.Validator")]
    public class ForgetPasswordCommandValidatorTests
    {
        private readonly ForgetPasswordCommandValidator _validator = new ForgetPasswordCommandValidator();

        [Theory]
        [InlineData(null)]                                              // email is null
        [InlineData("")]                                                // email is empty
        [InlineData("invalid-email-format")]                            // invalid email format 
        public void Validate_ShouldHaveError_WhenInvalidInputs(string email)
        {
            // Arrange
            var command = new ForgetPasswordCommand(email);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ShouldPass_WhenInputIsValid()
        {
            // Arrange
            var command = new ForgetPasswordCommand("test@gmail.com");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}

