using ExamSystem.Application.Settings;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ExamSystem.Infrastructure.Tests.Identity;

[Trait("Category", "Infrastructure.Identity.AccessToken")]
public class AccessTokenServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly AccessTokenService _service;
    private readonly IOptions<JWTSettings> _jwtSettingsOptions;

    public AccessTokenServiceTests()
    {
        _userManagerMock = MockHelper.CreateUserManagerMock<ApplicationUser>();

        _jwtSettingsOptions = Options.Create(new JWTSettings
        {
            SecretKey = "jsdjfsdfjfsdjfsdhksfdhsfdsdfjksjfsdk",
            Issuer = "Issuer",
            Audience = "Audience",
            DurationInMinutes = 30
        });

        _service = new AccessTokenService(_userManagerMock.Object, _jwtSettingsOptions);
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldReturnAccessTokenResponse()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-1",
            Email = "user@test.com"
        };

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Doctor" });

        // Act
        var result = await _service.GenerateTokenAsync(user);

        // Assert
        result.AccessToken.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.Role.Should().Be("Doctor");
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldContainCorrectClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-2",
            Email = "test@test.com"
        };

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Student" });

        // Act
        var response = await _service.GenerateTokenAsync(user);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);

        // Assert
        token.Claims.Should().ContainSingle(x => x.Type == ClaimTypes.Email && x.Value == user.Email);
        token.Claims.Should().ContainSingle(x => x.Type == ClaimTypes.Role && x.Value == "Student");
        token.Claims.Should().ContainSingle(x => x.Type == JwtRegisteredClaimNames.Sub && x.Value == "user-2");
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldCallGetRolesAsync()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = "user-3",
            Email = "x@test.com"
        };

        _userManagerMock.Setup(u => u.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        // Act
        await _service.GenerateTokenAsync(user);

        // Assert
        _userManagerMock.Verify(u => u.GetRolesAsync(user), Times.Once);
    }
}
