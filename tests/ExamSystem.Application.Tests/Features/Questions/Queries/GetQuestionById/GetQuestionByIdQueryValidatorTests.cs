using ExamSystem.Application.Features.Questions.Queries.GetQuestionById;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Features.Questions.Queries.GetQuestionById
{
    [Trait("Category", "Application.Question.GetQuestionById.Validator")]
    public class GetQuestionByIdQueryValidatorTests
    {
        private readonly GetQuestionByIdQueryValidator _validator = new GetQuestionByIdQueryValidator();
        private static GetQuestionByIdQuery CreateValidQuery() => new GetQuestionByIdQuery("doctor123", 1, 1);


        [Fact]
        public void Validate_ShouldHaveValidationError_WhenExamIdIsLessThanOrEqualZero()
        {
            //Arrange
            var query = CreateValidQuery() with { ExamId = 0 };

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(q => q.ExamId);
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenQuestionIdIsLessThanOrEqualZero()
        {
            //Arrange
            var query = CreateValidQuery() with { QuestionId = 0 };

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(q => q.QuestionId);
        }

        [Fact]
        public void Validate_ShouldNotHaveErrors_WhenQueryAreValid()
        {
            //Arrange
            var query = CreateValidQuery();

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
