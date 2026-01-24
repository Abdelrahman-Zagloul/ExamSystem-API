using ExamSystem.Application.Features.Authentication.Commands.ChangePassword;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Features.Authentication.Commands.ChangePassword
{
    [Trait("Layer", "Application")]
    [Trait("Feature", "Authentication")]
    [Trait("Action", "ChangePassword")]
    [Trait("Component", "Validator")]
    public class ChangePasswordCommandValidatorTests
    {
        private readonly ChangePasswordCommandValidator _validator = new ChangePasswordCommandValidator();

        [Theory]
        [InlineData(null, null)]            // Null
        [InlineData("", "")]                // Empty
        [InlineData("123", "1234567")]      // Too short
        [InlineData("ValidPass123", "")]    // New password missing
        [InlineData("", "ValidPass123")]    // Old password missing
        public void Validate_ShouldHaveError_WhenInvalidInputs(string currentPassword, string newPassword)
        {
            // Arrange
            var command = new ChangePasswordCommand("user-id", currentPassword, newPassword);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ShouldPass_WhenInputIsValid()
        {
            // Arrange
            var command = new ChangePasswordCommand("user-id", "currentPassword", "newPassword");

            //Act   
            var result = _validator.Validate(command);

            //Assert    
            result.IsValid.Should().BeTrue();
        }
    }
}
