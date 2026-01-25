using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion;
using ExamSystem.Application.Features.Questions.Commands.UpdateQuestion.Requests;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Questions.Commands.UpdateQuestion
{
    [Trait("Category", "Application.Question.Update.Validator")]
    public class UpdateQuestionCommandValidatorTests
    {
        private readonly UpdateQuestionCommandValidator _validator = new UpdateQuestionCommandValidator();

        private static UpdateQuestionCommand CreateValidCommand() =>
           new UpdateQuestionCommand(1, 1, "Valid text", 5, [new UpdateOptionRequest(1, "Option 1")], 1);

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdLessThanOrEqualZero()
        {
            // Arrange
            var command = CreateValidCommand() with { ExamId = 0 };

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
            var command = CreateValidCommand() with { QuestionId = 0 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionId);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionTextNotNullAndExceedMaxCharacter()
        {
            // Arrange
            var command = CreateValidCommand() with { QuestionText = new string('a', 10001) };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionText);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenNewQuestionMarkLessThanZero()
        {
            //Arrange
            var command = CreateValidCommand() with { NewQuestionMark = -1 };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.ShouldHaveValidationErrorFor(c => c.NewQuestionMark);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenOptionItemIsNull()
        {
            //Arrange
            var command = CreateValidCommand() with { Options = new List<UpdateOptionRequest>() { null } };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor("Options[0]");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenOptionIdIsLessThanOrEqualZero()
        {
            //Arrange
            var command = CreateValidCommand() with { Options = [new UpdateOptionRequest(0, "text")] };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor("Options[0].OptionId");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenOptionTextExceedsMaxLength()
        {
            //Arrange
            var command = CreateValidCommand() with { Options = [new UpdateOptionRequest(1, new string('x', 501))] };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor("Options[0].NewOptionText");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenNewCorrectOptionIdIsNotPositive()
        {
            //Arrange
            var command = CreateValidCommand() with { NewCorrectOptionId = 0 };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.ShouldHaveValidationErrorFor(c => c.NewCorrectOptionId);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationErrors_WhenCommandIsValid()
        {
            //Arrange
            var command = CreateValidCommand();

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
