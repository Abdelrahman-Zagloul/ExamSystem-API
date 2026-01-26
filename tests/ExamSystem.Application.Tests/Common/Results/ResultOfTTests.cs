using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using FluentAssertions;

namespace ExamSystem.Application.Tests.Common.Results
{
    [Trait("Category", "Application.Results.Result.Generic")]
    public class ResultOfTTests
    {
        [Fact]
        public void Ok_ShouldCreateSuccessfulResult_WithValue()
        {
            // Act
            var result = Result<string>.Ok("value");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("value");
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Ok_ShouldCreateSuccessfulResult_WithValueAndMessage()
        {
            // Act
            var result = Result<int>.Ok(10, "success");

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(10);
            result.Message.Should().Be("success");
        }

        [Fact]
        public void Fail_ShouldCreateFailedResult_WithSingleError()
        {
            // Arrange
            var error = Error.Validation("Id", "Invalid id");

            // Act
            var result = Result<int>.Fail(error);

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
                Error.Validation("Id", "Invalid id"),
                Error.Validation("Name", "Name is required")
            };

            // Act
            var result = Result<string>.Fail(errors);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Value_ShouldThrowException_WhenResultIsFailure()
        {
            // Arrange
            var error = Error.Validation("Id", "Invalid id");
            var result = Result<int>.Fail(error);

            // Act
            Action act = () => _ = result.Value;

            // Assert
            act.Should()
               .Throw<InvalidOperationException>()
               .WithMessage("The value of a failure result can not be accessed.");
        }

        [Fact]
        public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
        {
            // Act
            Result<string> result = "implicit value";

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("implicit value");
        }

        [Fact]
        public void ImplicitConversion_FromError_ShouldCreateFailedResult()
        {
            // Arrange
            var error = Error.Validation("Name", "Name is required");

            // Act
            Result<string> result = error;

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e == error);
        }

        [Fact]
        public void ImplicitConversion_FromErrorList_ShouldCreateFailedResult()
        {
            // Arrange
            var errors = new List<Error>
            {
                Error.Validation("Name", "Name is required")
            };

            // Act
            Result<int> result = errors;

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }
    }
}
