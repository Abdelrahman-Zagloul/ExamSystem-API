using ExamSystem.Application.Features.Questions.Command.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion;
using ExamSystem.Application.Features.Questions.Commands.CreateQuestion.Requests;
using ExamSystem.Domain.Entities.Questions;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Questions.Commands.CreateQuestion
{
    [Trait("Category", "Application.Question.Create.Validator")]
    public class CreateQuestionCommandValidatorTests
    {
        private readonly CreateQuestionCommandValidator _validator = new CreateQuestionCommandValidator();
        private static CreateQuestionCommand CreateValidCommand() =>
        new CreateQuestionCommand
        (
            ExamId: 1,
            QuestionText: "What is C#?",
            QuestionMark: 5,
            QuestionType: QuestionType.MCQ,
            Options:
            [
                new CreateOptionRequest("option1"),
                new CreateOptionRequest("option2"),
                new CreateOptionRequest("option3"),
                new CreateOptionRequest("option4")
            ],
            CorrectOptionNumber: 1
        );

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionTextIsEmpty()
        {
            // Arrange
            var command = CreateValidCommand() with { QuestionText = string.Empty };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionText);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionTextExceedsMaxLength()
        {
            // Arrange
            var command = CreateValidCommand() with { QuestionText = new string('t', 1001) };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionText);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionMarkLessThanOrEqualZero()
        {
            // Arrange
            var command = CreateValidCommand() with { QuestionMark = 0 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionMark);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionTypeIsInvalid()
        {
            // Arrange
            var command = CreateValidCommand() with { QuestionType = (QuestionType)100 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.QuestionType);
        }

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
        public void Validate_ShouldHaveValidationError_WhenOptionsIsEmpty()
        {
            // Arrange
            var command = CreateValidCommand() with { Options = [] };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.Options);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenOptionCountLessThanTwo()
        {
            // Arrange
            var command = CreateValidCommand() with { Options = [new CreateOptionRequest("option1")] };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.Options);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenCorrectOptionNumberIsZero()
        {
            // Arrange
            var command = CreateValidCommand() with { CorrectOptionNumber = 0 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.CorrectOptionNumber);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenCorrectOptionNumberExceedsOptionCount()
        {
            // Arrange
            var command = CreateValidCommand() with { CorrectOptionNumber = 5 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(c => c.CorrectOptionNumber);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationErrors_WhenCommandIsValid()
        {
            // Arrange
            var command = CreateValidCommand();
            // Act
            var result = _validator.TestValidate(command);
            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}




