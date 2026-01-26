using ExamSystem.Application.Features.Exams.Queries.GetExamByIdForDoctor;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Queries.GetExamByIdForDoctor
{
    [Trait("Category", "Application.Exam.GetExamByIdForDoctor.Validator")]
    public class GetExamByIdForDoctorQueryValidatorTests
    {
        private readonly GetExamByIdForDoctorQueryValidator _validator = new();

        [Fact]
        public async Task Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualToZero()
        {
            // Arrange
            var command = new GetExamByIdForDoctorQuery("doctor-id", 0);

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
            var command = new GetExamByIdForDoctorQuery("doctor-id", 1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
