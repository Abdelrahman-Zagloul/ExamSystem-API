using ExamSystem.Application.Features.Questions.Commands.DeleteQuestion;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Questions.Commands.DeleteQuestion
{
    [Trait("Category", "Application.Question.Delete.Validator")]
    public class DeleteQuestionCommandValidatorTests
    {
        private readonly DeleteQuestionCommandValidator _validator = new DeleteQuestionCommandValidator();

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdLessThanOrEqualZero()
        {
            // Arrange
            var command = new DeleteQuestionCommand(0, 1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.ExamId);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionIdLessThanOrEqualZero()
        {
            // Arrange
            var command = new DeleteQuestionCommand(1, 0);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionId);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = new DeleteQuestionCommand(1, 1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
