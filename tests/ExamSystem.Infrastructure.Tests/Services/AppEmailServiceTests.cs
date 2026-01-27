using ExamSystem.Application.Contracts.ExternalServices;
using ExamSystem.Application.Settings;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace ExamSystem.Infrastructure.Tests.Services
{
    [Trait("Category", "Infrastructure.Services.AppEmail")]
    public class AppEmailServiceTests
    {
        private readonly Mock<IMailService> _mailServiceMock;
        private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private readonly IOptions<FrontendURLsSettings> _uRLsSettingsOptions;
        private readonly AppEmailService _service;
        private static ApplicationUser CreateUser() => new ApplicationUser
        {
            FullName = "test",
            Email = "test@gmail.com"
        };
        private static string CreateTemplate(string fileName, string content)
        {
            var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var dir = Path.Combine(root, "EmailTemplates");
            Directory.CreateDirectory(dir);

            File.WriteAllText(Path.Combine(dir, fileName), content);
            return root;
        }
        public AppEmailServiceTests()
        {
            _mailServiceMock = new Mock<IMailService>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _uRLsSettingsOptions = Options.Create(new FrontendURLsSettings
            {
                ConfirmEmailPath = "https://path.com/confirm-email",
                ResetPasswordPath = "https://path.com/reset-password",
                ReviewExamResultPath = "https://path.com/review-exam/id"
            });
            _service = new AppEmailService(_mailServiceMock.Object, _webHostEnvironmentMock.Object, _uRLsSettingsOptions);
        }

        [Fact]
        public async Task SendEmailForWelcomeMessageAsync_ShouldSendEmailWithUserName()
        {
            // Arrange
            var root = CreateTemplate("WelcomeEmail.html", "Hello {FullName}");
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns(root);
            var user = CreateUser();

            // Act
            await _service.SendEmailForWelcomeMessageAsync(user);

            // Assert
            _mailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    user.Email!,
                    "Welcome to Exam System",
                    It.Is<string>(body => body.Contains("test")),
                    null),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailForConfirmEmailAsync_ShouldIncludeConfirmationLink()
        {
            // Arrange
            var root = CreateTemplate("ConfirmEmail.html", "{confirmationLink}");
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns(root);
            var user = CreateUser();

            // Act
            await _service.SendEmailForConfirmEmailAsync(user, "TOKEN123");

            // Assert
            _mailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    user.Email!,
                    "Confirm Email",
                    It.Is<string>(body =>
                        body.Contains("TOKEN123") &&
                        body.Contains("confirm")),
                    null),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailForPasswordChangedAsync_ShouldSendPasswordChangedEmail()
        {
            // Arrange
            var root = CreateTemplate("PasswordChanged.html", "Dear {FullName}, your password has been changed.");
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns(root);
            var user = CreateUser();

            // Act
            await _service.SendEmailForPasswordChangedAsync(user);

            //Assert
            _mailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    user.Email!,
                    "Password Changed Successfully",
                    It.Is<string>(body => body.Contains("test")),
                    null),
                Times.Once);

        }
        [Fact]
        public async Task SendEmailForResetPasswordAsync_ShouldIncludeResetLink()
        {
            // Arrange
            var root = CreateTemplate("ResetPassword.html", "{ResetLink}");
            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns(root);
            var user = new ApplicationUser
            {
                FullName = "Sara",
                Email = "sara@test.com"
            };

            // Act
            await _service.SendEmailForResetPasswordAsync(user, "RESET_TOKEN");

            // Assert
            _mailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    "sara@test.com",
                    "Reset Password",
                    It.Is<string>(body => body.Contains("RESET_TOKEN")),
                    null),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailForExamResultAsync_ShouldSendExamResultEmail()
        {
            // Arrange
            var root = CreateTemplate("ExamResultEmail.html", "{FullName}-{ExamTitle}-{Score}-{TotalMark}-{ReviewUrl}");

            _webHostEnvironmentMock.Setup(e => e.WebRootPath).Returns(root);

            // Act
            await _service.SendEmailForExamResultAsync(
                "Math",
                "Youssef",
                100,
                85,
                "y@test.com",
                5);

            // Assert
            _mailServiceMock.Verify(m =>
                m.SendEmailAsync(
                    "y@test.com",
                    "Exam Result - Math",
                    It.Is<string>(body =>
                        body.Contains("Youssef") &&
                        body.Contains("Math") &&
                        body.Contains("85") &&
                        body.Contains("100") &&
                        body.Contains("5")),
                    null),
                Times.Once);
        }

        // helper method to create temporary email template
    }
}
