using ExamSystem.Application.Common.Results.Errors;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Common.Results.Errors
{
    [Trait("Category", "Application.Results.Errors.Error")]
    public class ErrorTests
    {
        [Fact]
        public void BadRequest_ShouldCreateBadRequestError_WithDefaultValues()
        {
            // Act
            var error = Error.BadRequest();

            // Assert
            error.Title.Should().Be("BadRequest");
            error.Description.Should().Be("The request is invalid");
            error.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public void BadRequest_ShouldCreateBadRequestError_WithCustomValues()
        {
            // Act
            var error = Error.BadRequest("CustomTitle", "Custom description");

            // Assert
            error.Title.Should().Be("CustomTitle");
            error.Description.Should().Be("Custom description");
            error.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public void Validation_ShouldCreateValidationError_WithDefaultValues()
        {
            // Act
            var error = Error.Validation();

            // Assert
            error.Title.Should().Be("Validation Error");
            error.Description.Should().Be("One or more validation errors occurred");
            error.ErrorType.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Validation_ShouldCreateValidationError_WithCustomValues()
        {
            // Act
            var error = Error.Validation("Name", "Name is required");

            // Assert
            error.Title.Should().Be("Name");
            error.Description.Should().Be("Name is required");
            error.ErrorType.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public void Unauthorized_ShouldCreateUnauthorizedError()
        {
            // Act
            var error = Error.Unauthorized();

            // Assert
            error.Title.Should().Be("Unauthorized");
            error.ErrorType.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public void Forbidden_ShouldCreateForbiddenError()
        {
            // Act
            var error = Error.Forbidden();

            // Assert
            error.Title.Should().Be("Forbidden");
            error.ErrorType.Should().Be(ErrorType.Forbidden);
        }

        [Fact]
        public void NotFound_ShouldCreateNotFoundError()
        {
            // Act
            var error = Error.NotFound();

            // Assert
            error.Title.Should().Be("NotFound");
            error.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public void Conflict_ShouldCreateConflictError()
        {
            // Act
            var error = Error.Conflict();

            // Assert
            error.Title.Should().Be("Conflict");
            error.ErrorType.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public void Failure_ShouldCreateFailureError()
        {
            // Act
            var error = Error.Failure();

            // Assert
            error.Title.Should().Be("Failure");
            error.ErrorType.Should().Be(ErrorType.Failure);
        }

        [Fact]
        public void Error_RecordEquality_ShouldWorkCorrectly()
        {
            // Arrange
            var error1 = Error.Validation("Name", "Required");
            var error2 = Error.Validation("Name", "Required");

            //Assert
            error1.Should().Be(error2);
        }
    }
}
