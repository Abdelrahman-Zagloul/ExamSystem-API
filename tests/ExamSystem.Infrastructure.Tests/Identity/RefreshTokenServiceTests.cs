using ExamSystem.Application.Common.Results.Errors;
using ExamSystem.Application.Settings;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
using System.Linq.Expressions;

namespace ExamSystem.Infrastructure.Tests.Identity
{
    [Trait("Category", "Infrastructure.Identity.RefreshToken")]
    public class RefreshTokenServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IGenericRepository<RefreshToken>> _refreshTokenRepoMock;
        private readonly IOptions<RefreshTokenSettings> _refreshTokenSettingsOptions;
        private readonly RefreshTokenService _service;

        public RefreshTokenServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _refreshTokenRepoMock = new Mock<IGenericRepository<RefreshToken>>();
            _unitOfWorkMock.Setup(u => u.Repository<RefreshToken>()).Returns(_refreshTokenRepoMock.Object);
            _refreshTokenSettingsOptions = Options.Create(new RefreshTokenSettings()
            {
                RefreshTokenLifetimeDays = 7,
                RefreshTokenHashKey = "ewrwerweeevbxcbncvgjgh"
            });
            _service = new RefreshTokenService(_unitOfWorkMock.Object, _refreshTokenSettingsOptions);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAndPersistRefreshToken()
        {
            //Arrange
            var user = new ApplicationUser { Id = "user-1" };

            //Act
            var result = await _service.CreateAsync(user, "127.0.0.1", default);

            //Assert
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("user-1", result.UserId);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);

            _refreshTokenRepoMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), default), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RotateAsync_ShouldReturnBadRequestError_WhenTokenNotFound()
        {
            //Arrange
            _refreshTokenRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
                .ReturnsAsync((RefreshToken?)null);

            //Act
            var result = await _service.RotateAsync("invalid", null, default);

            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.BadRequest);
        }

        [Fact]
        public async Task RotateAsync_ShouldReturnBadRequestError_WhenTokenInactive()
        {
            //Arrange
            var token = new RefreshToken("user-id", "token-hash", DateTime.UtcNow.AddDays(-1), null);

            _refreshTokenRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
                .ReturnsAsync(token);

            //Act
            var result = await _service.RotateAsync("token", "ip", default);

            //Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorType == ErrorType.BadRequest);
        }

        [Fact]
        public async Task RotateAsync_ShouldRevokeOldToken_AndCreateNewOne()
        {
            //Arrange
            var token = new RefreshToken("user-id", "token-hash", DateTime.UtcNow.AddDays(1), null);

            _refreshTokenRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
                .ReturnsAsync(token);

            //Act
            var result = await _service.RotateAsync("token", "ip", default);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.UserId.Should().Be("user-id");
            _refreshTokenRepoMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), default), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RevokeAsync_ShouldRevokeToken_WhenActive()
        {
            //Arrange
            var token = new RefreshToken("user-id", "token-hash", DateTime.UtcNow.AddDays(1), null);

            _refreshTokenRepoMock.Setup(r => r.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>(), default))
                .ReturnsAsync(token);

            //Act
            var result = await _service.RevokeAsync("token", "ip", default);

            //Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task RevokeAllAsync_ShouldRevokeAllActiveTokens()
        {
            //Arrange
            var tokens = new List<RefreshToken>
            {
                new RefreshToken("user-1", "token-hash", DateTime.UtcNow.AddDays(1), null),
                new RefreshToken("user-1", "token-hash", DateTime.UtcNow.AddDays(2), null)
            };

            _refreshTokenRepoMock.Setup(r => r.GetAsQuery(false))
                .Returns(tokens.BuildMock());

            //Act
            await _service.RevokeAllAsync("user-1", "ip", default);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

    }
}
