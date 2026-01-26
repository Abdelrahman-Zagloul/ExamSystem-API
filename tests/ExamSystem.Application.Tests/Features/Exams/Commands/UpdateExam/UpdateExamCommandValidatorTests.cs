using ExamSystem.Application.Features.Exams.Commands.UpdateExam;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.UpdateExam
{
    [Trait("Category", "Application.Exam.Update.Validator")]
    public class UpdateExamCommandValidatorTests
    {
        private readonly UpdateExamCommandValidator _validator = new();
        private static UpdateExamCommand CreateEmptyCommand() =>
            new UpdateExamCommand(1, null, null, null, null, null);

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualToZero()
        {
            //Arrange
            var command = CreateEmptyCommand() with { ExamId = 0 };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.ExamId);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenTitleIsEmpty()
        {
            //Arrange
            var command = CreateEmptyCommand() with { Title = "  " };

            //Act
            var result = _validator.TestValidate(command);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
        [Fact]
        public void Validate_ShouldHaveValidationError_WhenTitleExceedsMaxLength()
        {
            //Arrange
            var command = CreateEmptyCommand() with { Title = new string('a', 201) };

            //Act   
            var result = _validator.TestValidate(command);

            //Assert  
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenDescriptionExceedsMaxLength()
        {
            //Arrange
            var command = CreateEmptyCommand() with { Description = new string('a', 1001) };

            //Act  
            var result = _validator.TestValidate(command);


            //Assert      
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenStartAtIsInThePast()
        {
            //Arrange
            var command = CreateEmptyCommand() with { StartAt = DateTime.UtcNow.AddMinutes(-10) };

            //Act
            var result = _validator.TestValidate(command);

            //Assert      
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.StartAt);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenDurationIsLessThanOrEqualToZero()
        {
            //Arrange
            var command = CreateEmptyCommand() with { DurationInMinutes = 0 };

            //Act
            var result = _validator.TestValidate(command);

            //Assert      
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.DurationInMinutes);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenEndAtIsLessThanStartAt()
        {
            //Arrange
            var command = CreateEmptyCommand() with
            {
                StartAt = DateTime.UtcNow.AddDays(2),
                EndAt = DateTime.UtcNow.AddDays(1)
            };

            //Act
            var result = _validator.TestValidate(command);

            //Assert      
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenDurationExceedsExamTimeRange()
        {
            //Arrange
            var command = CreateEmptyCommand() with
            {
                StartAt = DateTime.UtcNow.AddMinutes(10),
                EndAt = DateTime.UtcNow.AddMinutes(20),
                DurationInMinutes = 60
            };

            //Act
            var result = _validator.TestValidate(command);

            //Assert      
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenNoFieldsProvidedForUpdate()
        {
            //Arrange
            var command = CreateEmptyCommand();

            //Act
            var result = _validator.TestValidate(command);

            //Assert      
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenAtLeastOneValidFieldIsProvided()
        {
            var command = CreateEmptyCommand() with
            {
                Title = "Updated Title"
            };

            var result = _validator.TestValidate(command);

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
