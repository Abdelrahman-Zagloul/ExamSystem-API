using ExamSystem.Application.Features.ExamResults.Queries.GetExamResultsForDoctor;
using ExamSystem.Domain.Entities.Exams;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.ExamResults.Queries.GetExamResultsForDoctor
{
    [Trait("Category", "Application.Question.ExamResultsForDoctor.Validator")]
    public class GetExamResultsForDoctorQueryValidatorTests
    {
        private readonly GetExamResultsForDoctorQueryValidator _validator = new();
        private static GetExamResultsForDoctorQuery CreateValidQuery()
            => new GetExamResultsForDoctorQuery("doctor-id", 1, ExamResultStatus.Passed, 1, 10, "https://api/exams/1/restlts", new Dictionary<string, string>());

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualZero()
        {
            // Arrange
            var query = CreateValidQuery() with { ExamId = 0 };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(q => q.ExamId);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenStatusNotNullAndIsInvalidStatus()
        {
            // Arrange
            var query = CreateValidQuery() with { Status = (ExamResultStatus)100 };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(q => q.Status);
        }

        [Fact]
        public void Validate_ShouldNotHaveErrors_WhenQueryAreValid()
        {
            // Arrange
            var query = CreateValidQuery();

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }

    }
}
