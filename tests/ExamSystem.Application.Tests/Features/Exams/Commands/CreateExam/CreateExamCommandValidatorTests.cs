using ExamSystem.Application.Features.Exams.Commands.CreateExam;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.CreateExam
{
    [Trait("Category", "Application.Exam.Create.Validator")]
    public class CreateExamCommandValidatorTests
    {
        private readonly CreateExamCommandValidator _validator = new();
        private static CreateExamCommand CreateValidCommand() =>
            new CreateExamCommand("Title", "Description", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2), 60);

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenTitleIsEmpty()
        {
            // Arrange
            var command = CreateValidCommand() with { Title = string.Empty };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenTitleExceedsMaxLength()
        {
            // Arrange
            var command = CreateValidCommand() with { Title = new string('a', 201) };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenDescriptionIsNotNullAndExceedsMaxLength()
        {
            // Arrange
            var command = CreateValidCommand() with { Description = new string('a', 1001) };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenStartAtIsLessThanNow()
        {
            // Arrange
            var command = CreateValidCommand() with { StartAt = DateTime.UtcNow.AddDays(-1) };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.StartAt);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenEndAtIsLessThanStartAt()
        {
            // Arrange
            var command = CreateValidCommand() with { StartAt = DateTime.UtcNow.AddDays(1), EndAt = DateTime.UtcNow.AddDays(-1) };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.EndAt);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenDurationInMinutesIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = CreateValidCommand() with { DurationInMinutes = 0 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenDurationInMinutesIsGreaterThanDiffBetweenTheStartAtAndEndAt()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                DurationInMinutes = 1000,
                StartAt = DateTime.UtcNow.AddMinutes(10),
                EndAt = DateTime.UtcNow.AddMinutes(20)
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes);
        }

        [Fact]
        public void Validate_ShouldNotHaveError_WhenCommandIsValid()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}


