using ExamSystem.Application.Common.Interfaces;
using ExamSystem.Application.Common.Validations;
using FluentValidation.TestHelper;

namespace ExamSystem.Application.Tests.Common.Validators
{
    [Trait("Category", "Application.Common.Validators.PaginationValidator")]
    public class PaginationValidatorTests
    {
        private record FakePaginatedQuery(int PageNumber, int PageSize, string BaseUrl, Dictionary<string, string> QueryParams) : IPaginatedQuery;

        private readonly PaginationValidator<FakePaginatedQuery> _validator = new();
        private static FakePaginatedQuery CreateValidQuery()
            => new(1, 10, "https://example.com", new Dictionary<string, string>());

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenPageNumberIsLessThanOrEqualZero()
        {
            // Arrange
            var query = CreateValidQuery() with { PageNumber = 0 };

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber)
                  .WithErrorMessage("Page number must be greater than 0.");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenPageSizeIsGreaterThan50()
        {
            //Arrange
            var query = CreateValidQuery() with { PageSize = 51 };

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize)
                  .WithErrorMessage("Page size must be less than 50");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenBaseUrlIsEmpty()
        {
            //Arrange
            var query = CreateValidQuery() with { BaseUrl = string.Empty };

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.BaseUrl)
                  .WithErrorMessage("BaseUrl is required.");
        }

        [Fact]
        public void Validate_ShouldHaveValidationError_WhenBaseUrlIsNotValidAbsoluteUrl()
        {
            //Arrange
            var query = CreateValidQuery() with { BaseUrl = "not-a-valid-url" };

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.ShouldHaveValidationErrorFor(x => x.BaseUrl)
                  .WithErrorMessage("BaseUrl must be a valid absolute URL.");
        }

        [Fact]
        public void Validate_ShouldNotHaveValidationErrors_WhenQueryIsValid()
        {
            //Arrange
            var query = CreateValidQuery();

            //Act
            var result = _validator.TestValidate(query);

            //Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
