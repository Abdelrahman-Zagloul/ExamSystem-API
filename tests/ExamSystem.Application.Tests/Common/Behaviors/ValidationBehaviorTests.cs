using ExamSystem.Application.Common.Behaviors;
using ExamSystem.Application.Common.Results;
using FluentAssertions;
using FluentValidation;
using MediatR;

namespace ExamSystem.Application.Tests.Common.Behaviors
{

    [Trait("Category", "Application.Behaviors.ValidationBehavior")]
    public class ValidationBehaviorTests
    {
        private record TestCommandV1(string Name) : IRequest<Result>;
        private record TestCommandV2(string Name) : IRequest<Result<string>>;

        [Fact]
        public async Task Handle_ShouldCallNext_WhenNoValidatorsForNoneGenericResult()
        {
            // Arrange
            var behavior = new ValidationBehavior<TestCommandV1, Result>(Enumerable.Empty<IValidator<TestCommandV1>>());
            var nextCalled = false;
            RequestHandlerDelegate<Result> next = async _ =>
            {
                nextCalled = true;
                return Result.Ok();
            };
            var command = new TestCommandV1("Exam 1");

            // Act
            var result = await behavior.Handle(
                command,
                next,
                CancellationToken.None
            );

            // Assert
            nextCalled.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenValidationSucceedsForGenericResult()
        {
            // Arrange
            var validator = new InlineValidator<TestCommandV2>();
            validator.RuleFor(x => x.Name).NotEmpty();
            var behavior = new ValidationBehavior<TestCommandV2, Result<string>>(new[] { validator });
            var nextCalled = false;
            RequestHandlerDelegate<Result<string>> next = async _ =>
            {
                nextCalled = true;
                return Result<string>.Ok("Success");
            };

            var command = new TestCommandV2("Valid Name");

            // Act
            var result = await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            nextCalled.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be("Success");
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenValidationFailsForNoneGenericResult()
        {
            // Arrange
            var validator = new InlineValidator<TestCommandV1>();
            validator.RuleFor(x => x.Name).NotEmpty();

            var behavior = new ValidationBehavior<TestCommandV1, Result>(new[] { validator });
            var nextCalled = false;

            RequestHandlerDelegate<Result> next = async _ =>
            {
                nextCalled = true;
                return Result.Ok();
            };

            var command = new TestCommandV1("");

            // Act
            var result = await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            nextCalled.Should().BeFalse();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenValidationFailsForGenericResult()
        {
            // Arrange
            var validator = new InlineValidator<TestCommandV2>();
            validator.RuleFor(x => x.Name).NotEmpty();
            var behavior = new ValidationBehavior<TestCommandV2, Result<string>>(new[] { validator });
            var nextCalled = false;
            RequestHandlerDelegate<Result<string>> next = async _ =>
            {
                nextCalled = true;
                return Result<string>.Ok("Success");
            };

            var command = new TestCommandV2("");

            // Act
            var result = await behavior.Handle(command, next, CancellationToken.None);

            // Assert
            nextCalled.Should().BeFalse();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle();
        }

    }
}
