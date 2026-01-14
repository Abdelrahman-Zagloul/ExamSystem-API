using ExamSystem.Application.Features.Authentication.Commands.ResetPassword;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandValidatorTests
    {
        private readonly ResetPasswordCommandValidator _validator = new ResetPasswordCommandValidator();

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("invalid-email")]
        public void Validate_ShouldHaveError_WhenEmailIsInvalid(string email)
        {
            // Arrange
            var command = new ResetPasswordCommand(email, "validToken", "ValidPass123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Validate_ShouldHaveError_WhenTokenIsInvalid(string token)
        {
            // Arrange
            var command = new ResetPasswordCommand("email@gmail.com", token, "ValidPass123");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("short")]
        public void Validate_ShouldHaveError_WhenNewPasswordIsInvalid(string newPassword)
        {
            // Arrange
            var command = new ResetPasswordCommand("email@gmail.com", "validToken", newPassword);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }



        [Fact]
        public void Validate_ShouldPass_WhenInputIsValid()
        {
            // Arrange
            var command = new ResetPasswordCommand("email@gmail.com", "token", "newPassword");

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
