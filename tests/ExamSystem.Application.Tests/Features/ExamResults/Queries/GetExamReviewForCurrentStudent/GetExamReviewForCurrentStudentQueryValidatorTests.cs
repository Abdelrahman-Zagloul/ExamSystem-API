using ExamSystem.Application.Features.ExamResults.Queries.GetExamReviewForCurrentStudent;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.ExamResults.Queries.GetExamReviewForCurrentStudent
{
    [Trait("Category", "Application.Question.ExamReview.Validator")]
    public class GetExamReviewForCurrentStudentQueryValidatorTests
    {
        private readonly GetExamReviewForCurrentStudentQueryValidator _validator = new();

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualZero()
        {
            // Arrange
            var query = new GetExamReviewForCurrentStudentQuery("student-id", 0);

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(x => x.ExamId);
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationError_WhenQueryIsValid()
        {
            // Arrange
            var query = new GetExamReviewForCurrentStudentQuery("student-id", 1);

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(x => x.ExamId);
        }
    }
}
