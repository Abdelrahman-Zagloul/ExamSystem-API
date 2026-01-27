using ExamSystem.API.Controllers.V1;
using ExamSystem.Application.Common.Results;
using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Features.Authentication.Commands.ChangePassword;
using ExamSystem.Application.Features.Authentication.Commands.ChangePassword.Requests;
using ExamSystem.Application.Features.Authentication.Commands.ConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ForgetPassword;
using ExamSystem.Application.Features.Authentication.Commands.Login;
using ExamSystem.Application.Features.Authentication.Commands.Login.Requests;
using ExamSystem.Application.Features.Authentication.Commands.Logout;
using ExamSystem.Application.Features.Authentication.Commands.RefreshToken;
using ExamSystem.Application.Features.Authentication.Commands.Register;
using ExamSystem.Application.Features.Authentication.Commands.ResendConfirmEmail;
using ExamSystem.Application.Features.Authentication.Commands.ResetPassword;
using ExamSystem.Application.Features.Authentication.Commands.RevokeToken;
using ExamSystem.Application.Features.Authentication.Shared;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace ExamSystem.API.Tests.Controllers.V1
{
    [Trait("Category", "API.Controller.V1.Authentication")]
    public class AuthenticationControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthenticationController(_mediatorMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.HttpContext.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnOk_WhenMediatorSuccessAndSetCookie()
        {
            // Arrange
            var request = new LoginRequest("test@test.com", "Test12345");

            var result = Result<AccessWithRefreshTokenDto>
                .Ok(
                    new AccessWithRefreshTokenDto(
                        new AccessTokenResponse("access", "role", "user-id", DateTime.UtcNow.AddMinutes(20)),
                        new RefreshTokenDto("refresh", DateTime.UtcNow.AddDays(7), "user-id")
                    )
                );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.LoginAsync(request);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _controller.Response.Headers.Should().ContainKey("Set-Cookie");
        }

        [Fact]
        public async Task LoginAsync_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            var request = new LoginRequest("test@test.com", "WrongPassword");
            var result = Result<AccessWithRefreshTokenDto>.Fail(Error.Unauthorized("Invalid credentials", ""));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.LoginAsync(request);

            // Assert
            response.Should().BeOfType<ObjectResult>();
            _controller.Response.Headers.Should().NotContainKey("Set-Cookie");
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            var command = new RegisterCommand("fullname", "test@test.com", "Test12345", "Test12345", RoleDto.Student);
            var result = Result.Ok("Registered successfully");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.RegisterAsync(command);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ConfirmEmailAsync_ShouldReturnOk_WhenMediatorSuccessAndSetCookie()
        {
            // Arrange
            var result = Result<AccessWithRefreshTokenDto>
                .Ok(
                    new AccessWithRefreshTokenDto(
                        new AccessTokenResponse("access", "role", "user-id", DateTime.UtcNow.AddMinutes(20)),
                        new RefreshTokenDto("refresh", DateTime.UtcNow.AddDays(7), "user-id")
                    )
                );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ConfirmEmailCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.ConfirmEmailAsync("test@test.com", "token");

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _controller.Response.Headers.Should().ContainKey("Set-Cookie");
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnOk_WhenMediatorSuccessAndRenewCookie()
        {
            // Arrange            
            _controller.HttpContext.Request.Headers.Add("Cookie", "refreshToken=old-refresh-token");

            var result = Result<AccessWithRefreshTokenDto>
                .Ok(
                    new AccessWithRefreshTokenDto(
                        new AccessTokenResponse("new-access", "role", "user-id", DateTime.UtcNow.AddMinutes(20)),
                        new RefreshTokenDto("new-refresh", DateTime.UtcNow.AddDays(7), "user-id")
                    )
                );

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.RefreshTokenAsync();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _controller.Response.Headers.Should().ContainKey("Set-Cookie");
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            var result = Result<AccessWithRefreshTokenDto>
                .Fail(Error.BadRequest("refresh token invalid", ""));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RefreshTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.RefreshTokenAsync();

            // Assert
            response.Should().BeOfType<ObjectResult>();
            _controller.Response.Headers.Should().NotContainKey("Set-Cookie");
        }

        [Fact]
        public async Task RevokeTokenAsync_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            _controller.HttpContext.Request.Headers.Add("Cookie", "refreshToken=refresh-token");
            var result = Result.Ok();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RevokeTokenCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            _controller.HttpContext.Response.Headers.Remove("refreshToken");

            // Act
            var response = await _controller.RevokeTokenAsync();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ResendConfirmEmailAsync_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            var command = new ResendConfirmEmailCommand("test@test.com");
            var result = Result.Ok("Email sent");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.ResendConfirmEmailAsync(command);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturnOk_WhenMediatorSuccessAndRemoveCookie()
        {
            // Arrange
            _controller.HttpContext.Request.Headers.Add("Cookie", "refreshToken=refresh-token");

            var result = Result.Ok();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LogoutCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.LogoutAsync();

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            _controller.Response.Headers.Should().ContainKey("Set-Cookie");
        }

        [Fact]
        public async Task ChangePasswordAsync_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            var dto = new ChangePasswordRequest("Old12345", "New12345");
            var result = Result.Ok("Password changed");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Fake authenticated user
            _controller.HttpContext.User = new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, "user-id")], "TestAuth"));

            // Act
            var response = await _controller.ChangePasswordAsync(dto);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ForgetPasswordAsync_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            var command = new ForgetPasswordCommand("test@test.com");
            var result = Result.Ok("Reset link sent");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.ForgetPasswordAsync(command);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task ResetPasswordAsync_ShouldReturnOk_WhenMediatorSuccess()
        {
            // Arrange
            var command = new ResetPasswordCommand("test@test.com", "token", "NewPassword12345");

            var result = Result.Ok("Password reset");

            _mediatorMock
                .Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.ResetPasswordAsync(command);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AnyEndpoint_ShouldHandleResult_WhenMediatorFails()
        {
            // Arrange
            var request = new LoginRequest("x@y.com", "bad");
            var result = Result<AccessWithRefreshTokenDto>.Fail(Error.BadRequest("fail", ""));

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var response = await _controller.LoginAsync(request);

            // Assert
            response.Should().BeOfType<ObjectResult>();
        }

    }
}
