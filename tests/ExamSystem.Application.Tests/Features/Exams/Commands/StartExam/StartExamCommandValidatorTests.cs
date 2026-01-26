using ExamSystem.Application.Features.Exams.Commands.StartExam;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.StartExam
{
    [Trait("Category", "Application.Exam.StartExam.Validator")]
    public class StartExamCommandValidatorTests
    {
        private readonly StartExamCommandValidator _validator = new();

        [Fact]
        public async Task Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = new StartExamCommand("student-id", 0);

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
            var command = new StartExamCommand("student-id", 1);


            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
