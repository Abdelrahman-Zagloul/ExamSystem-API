using ExamSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExamSystem.Infrastructure.Tests.Identity;

[Trait("Category", "Infrastructure.Identity.CurrentUser")]

public class CurrentUserServiceTests
{
    private static CurrentUserService CreateServiceWithClaims(IEnumerable<Claim> claims, bool isAuthenticated = true)
    {
        var identity = new ClaimsIdentity(claims, isAuthenticated ? "TestAuth" : null);
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        var accessor = new HttpContextAccessor { HttpContext = context };

        return new CurrentUserService(accessor);
    }

    [Fact]
    public void UserId_ShouldReturnUserId_WhenClaimExists()
    {
        //Arrange
        var service = CreateServiceWithClaims(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-1")
        });

        //Act & Assert
        Assert.Equal("user-1", service.UserId);
    }

    [Fact]
    public void Email_ShouldReturnEmail_WhenClaimExists()
    {
        //Arrange
        var service = CreateServiceWithClaims(new[]
        {
            new Claim(ClaimTypes.Email, "test@test.com")
        });

        //Act & Assert
        Assert.Equal("test@test.com", service.Email);
    }

    [Fact]
    public void IsAuthenticated_ShouldReturnTrue_WhenUserAuthenticated()
    {
        //Arrange
        var service = CreateServiceWithClaims(Array.Empty<Claim>(), true);

        //Act & Assert
        Assert.True(service.IsAuthenticated);
    }

    [Fact]
    public void IsAuthenticated_ShouldReturnFalse_WhenUserNotAuthenticated()
    {
        //Arrange
        var service = CreateServiceWithClaims(Array.Empty<Claim>(), false);

        //Act & Assert
        Assert.False(service.IsAuthenticated);
    }

    [Fact]
    public void IsInRole_ShouldReturnTrue_WhenUserInRole()
    {
        //Arrange
        var service = CreateServiceWithClaims(new[]
        {
            new Claim(ClaimTypes.Role, "Admin")
        });

        //Act & Assert
        Assert.True(service.IsInRole("Admin"));
    }

    [Fact]
    public void Roles_ShouldReturnAllUserRoles()
    {
        //Arrange
        var service = CreateServiceWithClaims(new[]
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.Role, "Doctor")
        });

        //Act 
        var roles = service.Roles.ToList();

        //Assert
        Assert.Equal(2, roles.Count);
        Assert.Contains("Admin", roles);
        Assert.Contains("Doctor", roles);
    }

    [Fact]
    public void Properties_ShouldReturnNullOrFalse_WhenNoHttpContext()
    {
        //Arrange
        var accessor = new HttpContextAccessor { HttpContext = null };
        var service = new CurrentUserService(accessor);

        //Act & Assert
        Assert.Null(service.UserId);
        Assert.Null(service.Email);
        Assert.False(service.IsAuthenticated);
        Assert.Empty(service.Roles);
    }
}
