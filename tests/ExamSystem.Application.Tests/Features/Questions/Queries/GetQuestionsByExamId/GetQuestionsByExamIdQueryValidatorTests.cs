using ExamSystem.Application.Features.Questions.Queries.GetQuestionsByExamId;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Questions.Queries.GetQuestionsByExamId
{
    [Trait("Category", "Application.Question.GetQuestionByExamId.Validator")]
    public class GetQuestionsByExamIdQueryValidatorTests
    {
        private readonly GetQuestionsByExamIdQueryValidator _validator = new GetQuestionsByExamIdQueryValidator();
        private static GetQuestionsByExamIdQuery CreateValidQuery()
            => new GetQuestionsByExamIdQuery("doctor-Id", 1, 1, 1, "base-url", new Dictionary<string, string>());

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

    }
}
