using ExamSystem.Application.Features.Authentication.Commands.Register;
using ExamSystem.Application.Features.Authentication.DTOs;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Authentication.Commands.Register
{
    public class RegisterCommandValidatorTests
    {
        private readonly RegisterCommandValidator _validator = new RegisterCommandValidator();

        [Theory]
        [InlineData("", "test@example.com", "Password1", "Password1", RoleDto.Student)]             // Empty FullName
        [InlineData("test", "", "Password1", "Password1", RoleDto.Student)]                         // Empty Email
        [InlineData("test", "invalid-email", "Password1", "Password1", RoleDto.Student)]            // Invalid Email format
        [InlineData("test", "test@example.com", "", "", RoleDto.Student)]                           // Empty Password & Confirm
        [InlineData("test", "test@example.com", "short", "short", RoleDto.Student)]                 // Password too short
        [InlineData("test", "test@example.com", "password", "password", RoleDto.Student)]           // Password missing number
        [InlineData("test", "test@example.com", "PASSWORD1", "PASSWORD1", RoleDto.Student)]         // Password missing lowercase
        [InlineData("test", "test@example.com", "Password1", "Mismatch1", RoleDto.Student)]         // ConfirmPassword mismatch
        public void RegisterCommandValidator_ShouldHaveError_WhenInvalidInputs
            (string fullName, string email, string password, string confirmPassword, RoleDto role)
        {
            // Arrange
            var command = new RegisterCommand(fullName, email, password, confirmPassword, role);
            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void RegisterCommandValidator_ShouldPass_WhenInputIsValid()
        {
            // Arrange
            var command = new RegisterCommand("test", "test@example.com", "Password1", "Password1", RoleDto.Student);

            //Act   
            var result = _validator.Validate(command);

            //Assert    
            result.IsValid.Should().BeTrue();
        }
    }
}
