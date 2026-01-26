using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Common.Results
{
    [Trait("Category", "Application.Results.Result")]
    public class ResultTests
    {
        [Fact]
        public void Ok_ShouldCreateSuccessfulResult()
        {
            // Act
            var result = Result.Ok();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.Message.Should().BeNull();
        }

        [Fact]
        public void Ok_ShouldSetMessage_WithMessageAdd()
        {
            // Act
            var result = Result.Ok("Success message");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Success message");
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Fail_ShouldCreateFailedResult_WithSingleError()
        {
            // Arrange
            var error = Error.Validation("Name", "Name is required");

            // Act
            var result = Result.Fail(error);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].Should().Be(error);
        }

        [Fact]
        public void Fail_ShouldCreateFailedResult_WithErrorList()
        {
            // Arrange
            var errors = new List<Error>
            {
                Error.Validation("Name", "Name is required"),
                Error.Validation("Duration", "Duration must be greater than zero")
            };

            // Act
            var result = Result.Fail(errors);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void ImplicitConversion_ShouldCreateSuccessResult_FromString()
        {
            // Act
            Result result = "test";

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("test");
        }

        [Fact]
        public void ImplicitConversion_ShouldCreateFailedResult_FromError()
        {
            // Arrange
            var error = Error.Validation("Name", "Name is required");

            // Act
            Result result = error;

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e == error);
        }

        [Fact]
        public void ImplicitConversion_ShouldCreateFailedResult_FromErrorList()
        {
            // Arrange
            var errors = new List<Error>
            {
                Error.Validation("Name", "Name is required")
            };

            // Act
            Result result = errors;

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }
    }
}
