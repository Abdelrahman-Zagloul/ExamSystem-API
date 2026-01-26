using ExamSystem.Application.Features.Exams.Queries.GetExamsForStudent;
using ExamSystem.Domain.Entities.Exams;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Exams.Queries.GetExamsForStudent
{
    [Trait("Category", "Application.Exam.GetExamsForStudent.Validator")]
    public class GetExamsForStudentQueryValidatorTests
    {
        private readonly GetExamsForStudentQueryValidator _validator = new();

        private static GetExamsForStudentQuery CreateValidQuery() =>
             new GetExamsForStudentQuery(null, 1, 20, "https://path", new Dictionary<string, string>());

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamStatusIsInvalidEnumValue()
        {
            // Arrange
            var query = CreateValidQuery() with { ExamStatus = (ExamStatus)10 };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.ExamStatus);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenExamStatusIsNull()
        {
            // Arrange
            var query = CreateValidQuery() with { ExamStatus = null };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(x => x.ExamStatus);
        }

        [Fact]
        public void Validate_ShouldNotHaveAnyValidationErrors_WhenQueryIsValid()
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


