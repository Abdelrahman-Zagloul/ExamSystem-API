using ExamSystem.Application.Features.Exams.Commands.DeleteExam;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.DeleteExam
{
    [Trait("Category", "Application.Exam.Delete.Validator")]
    public class DeleteExamCommandValidatorTests
    {
        private readonly DeleteExamCommandValidator _validator = new();

        [Fact]
        public async Task Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = new DeleteExamCommand(0);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.ExamId);
        }
        [Fact]
        public void Validate_ShouldNotHaveError_WhenCommandIsValid()
        {
            // Arrange
            var command = new DeleteExamCommand(1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
