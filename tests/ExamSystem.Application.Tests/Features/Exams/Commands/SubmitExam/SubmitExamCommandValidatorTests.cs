using ExamSystem.Application.Features.Exams.Commands.SubmitExam;
using ExamSystem.Application.Features.Exams.Commands.SubmitExam.Requests;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Commands.SubmitExam
{
    [Trait("Category", "Application.Exam.SubmitExam.Validator")]
    public class SubmitExamCommandValidatorTests
    {
        private readonly SubmitExamCommandValidator _validator = new();

        private static SubmitExamCommand CreateValidCommand()
        {
            return new SubmitExamCommand("student-id", 1, new List<SubmitAnswerRequest>
                {
                    new SubmitAnswerRequest
                    {
                        QuestionId = 1,
                        SelectedOptionId = 2
                    }
                }
            );
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = CreateValidCommand() with { ExamId = 0 };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.ExamId);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenAnswersContainNull()
        {
            // Arrange
            var command = CreateValidCommand() with { Answers = new List<SubmitAnswerRequest> { null } };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor("Answers[0]");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionIdInAnswersIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Answers = new List<SubmitAnswerRequest> {
                    new SubmitAnswerRequest { QuestionId = 0, SelectedOptionId = 1 }
                }
            };
            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor("Answers[0].QuestionId");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenSelectedOptionIdInAnswersIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = CreateValidCommand() with
            {
                Answers = new List<SubmitAnswerRequest> {
                    new SubmitAnswerRequest { QuestionId = 1, SelectedOptionId = 0 }
                }
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor("Answers[0].SelectedOptionId");
        }

        [Fact]
        public void Validate_ShouldNotHaveAnyValidationErrors_WhenCommandIsValid()
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
