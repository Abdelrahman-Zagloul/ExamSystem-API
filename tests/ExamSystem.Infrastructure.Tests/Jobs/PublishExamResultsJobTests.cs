using ExamSystem.Application.Contracts.Services;
using ExamSystem.Application.Tests.Helpers;
using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Users;
using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Jobs;
using MockQueryable;
using Moq;

namespace ExamSystem.Infrastructure.Tests.Jobs
{
    [Trait("Category", "Infrastructure.Jobs.PublishExamResults")]
    public class PublishExamResultsJobTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IAppEmailService> _appEmailServiceMock;
        private readonly Mock<IGenericRepository<Exam>> _examRepoMock;
        private readonly Mock<IGenericRepository<ExamResult>> _examResultRepoMock;
        private readonly PublishExamResultsJob _job;
        private record ExamResultDetails(string ExamTitle, string StudentName, string Email, double TotalMark, double Score);
        public PublishExamResultsJobTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _appEmailServiceMock = new Mock<IAppEmailService>();
            _examRepoMock = new Mock<IGenericRepository<Exam>>();
            _examResultRepoMock = new Mock<IGenericRepository<ExamResult>>();
            _unitOfWorkMock.Setup(x => x.Repository<Exam>()).Returns(_examRepoMock.Object);
            _unitOfWorkMock.Setup(x => x.Repository<ExamResult>()).Returns(_examResultRepoMock.Object);
            _job = new PublishExamResultsJob(_unitOfWorkMock.Object, _appEmailServiceMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn_WhenExamNotFound()
        {
            // Arrange
            _examRepoMock
                .Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), It.IsAny<object[]>()))
                .ReturnsAsync((Exam?)null);

            //Act
            await _job.ExecuteAsync(1);

            //Assert
            _appEmailServiceMock.Verify(
                e => e.SendEmailForExamResultAsync(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<double>(), It.IsAny<double>(),
                    It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn_WhenResultsAlreadyPublished()
        {
            //Arrange
            var exam = new Exam { Id = 1 };
            exam.PublishExamResults();

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), It.IsAny<object[]>()))
                .ReturnsAsync(exam);

            //Act
            await _job.ExecuteAsync(1);

            //Assert
            _appEmailServiceMock.Verify(
                e => e.SendEmailForExamResultAsync(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<double>(), It.IsAny<double>(),
                    It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturn_WhenNoExamResults()
        {
            //Arrange
            var exam = new Exam { Id = 1, Title = "test" };

            _examRepoMock.Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), It.IsAny<object[]>()))
                .ReturnsAsync(exam);

            var results = new List<ExamResult>().BuildMock();
            _examResultRepoMock.Setup(r => r.GetAsQuery(true))
                .Returns(results);

            //Act
            await _job.ExecuteAsync(1);

            //Assert
            _appEmailServiceMock.Verify(
                e => e.SendEmailForExamResultAsync(
                    It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<double>(), It.IsAny<double>(),
                    It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSendEmails_AndPublishResults()
        {
            var exam = new Exam { Id = 1, Title = "test" };

            _examRepoMock
                .Setup(r => r.FindAsync(It.IsAny<CancellationToken>(), It.IsAny<object[]>()))
                .ReturnsAsync(exam);

            var examResult1 = new ExamResult("student-id", 1, 100, 80);
            var examResult2 = new ExamResult("student-id", 1, 100, 90);
            PrivatePropertySetter.Set(examResult1, "Student", new Student { Id = "student-id" });
            PrivatePropertySetter.Set(examResult2, "Student", new Student { Id = "student-id" });

            var results = new List<ExamResult> { examResult1, examResult2 }.BuildMock();

            _examResultRepoMock.Setup(r => r.GetAsQuery(true))
                .Returns(results);

            // Act
            await _job.ExecuteAsync(1);

            // Assert
            Assert.True(exam.ResultsPublished);
            _appEmailServiceMock.Verify(
                e => e.SendEmailForExamResultAsync(
                    "test",
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<string>(),
                    1),
                Times.Exactly(2));

            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

    }
}
